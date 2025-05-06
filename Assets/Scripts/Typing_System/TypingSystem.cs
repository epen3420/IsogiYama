using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using IsogiYama.System;
using SoundSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TypingSystem : MonoBehaviour
{
    private GameFlowManager gameFlowManager;
    private SoundPlayer soundPlayer;
    private VFXController vfxController; // VFX用クラス
    private TypingJudger typingJudger; // タイピング判定用クラス
    private CsvData<TypingQuestType> questList; // 問題を格納するリスト
    private int questIndex = 0;
    private List<BGInfo> bGInfos = new List<BGInfo>();
    private int bgIndex = 0;

    private struct BGInfo
    {
        public string imagePath;
        public float time;
    }

    [SerializeField]
    private StopwatchTimer timer;
    [SerializeField]
    private TMP_Text japaneseText;
    [SerializeField]
    private TMP_Text romaText;
    [SerializeField]
    private TMP_Text inputText;


    private void OnEnable()
    {
        EnableKeyboardInput();
    }

    private void OnDisable()
    {
        DisableKeyboardInput();
    }

    private void Start()
    {
        gameFlowManager = GameFlowManager.instance;
        soundPlayer = SoundPlayer.instance;
        vfxController = InstanceRegister.Get<VFXController>(); // VFX用クラスのインスタンスを取得

        var csvLoader = new CSVLoader();
        var csvFile = gameFlowManager.GetCurrentCSV();
        questList = csvLoader.LoadCSV<TypingQuestType>(csvFile);

        var firstImagePath = questList.Rows[0].Get<string>(TypingQuestType.image0);
        vfxController.ChangeBackgroundAsync(firstImagePath, 0.0f).Forget(); // 最初の背景を設定

        for (int i = 1; (TypingQuestType)i != TypingQuestType.japanese; i += 2)
        {
            var bgInfo = new BGInfo();
            try
            {
                bgInfo.imagePath = questList.Rows[0].Get<string>((TypingQuestType)i);
                bgInfo.time = float.Parse(questList.Rows[0].Get<string>((TypingQuestType)i + 1));
            }
            catch
            {
                continue;
            }
            Debug.Log($"Image Path: {bgInfo.imagePath}, Time: {bgInfo.time}");

            if (bgInfo.imagePath != null)
            {
                bGInfos.Add(bgInfo);
            }
        }

        Init();
    }

    private void Init()
    {
        inputText.maxVisibleCharacters = 0;

        if (questIndex >= questList.Rows.Count)
        {
            timer.StopTimer();
            DisableKeyboardInput();

            var clearTime = timer.GetTime();
            Debug.Log($"Clear Time: {clearTime}");

            gameFlowManager.SetClearTime(clearTime);

            timer.ResetTimer();

            japaneseText.text = "Game Clear";
            romaText.text = null;
            inputText.text = null;

            gameFlowManager.GoToNextScene();

            return;
        }

        var currentQuest = questList.Rows[questIndex++];
        var currentJapanese = currentQuest.Get<string>(TypingQuestType.japanese);
        var currentRoma = currentQuest.Get<string>(TypingQuestType.roma);

        japaneseText.text = currentJapanese;
        romaText.text = currentRoma;
        inputText.text = currentRoma;

        timer.StartTimer();
        typingJudger = new TypingJudger(currentRoma);
        Debug.Log($"Current Quest: {currentRoma}");
    }

    private void Update()
    {
        if (bgIndex >= bGInfos.Count) return;

        if (bGInfos[bgIndex].time < timer.GetTime())
        {
            Debug.Log($"Change Background: {bGInfos[bgIndex].imagePath}");
            vfxController.ChangeBackgroundAsync(bGInfos[bgIndex].imagePath).Forget();
            bgIndex++;
        }
    }

    /// <summary>
    /// タイピングの入力を受け取り、判定を行う
    /// </summary>
    /// <param name="typedChar"></param>
    private void OnKeyboardInput(char typedChar)
    {
        switch (typingJudger.JudgeChar(typedChar))
        {
            case TypingState.Hit:
                inputText.maxVisibleCharacters++;
                soundPlayer.PlaySe("TypeHit");

                Debug.Log($"{typedChar}: Hit");
                break;

            case TypingState.Miss:
                Debug.Log($"{typedChar}: Miss");
                soundPlayer.PlaySe("TypeMiss");
                break;

            case TypingState.Clear:
                inputText.maxVisibleCharacters++;
                soundPlayer.PlaySe("TypeHit");

                Debug.Log($"{typedChar}: Clear");

                Init();
                break;

            default:
                Debug.Log("Error");
                break;
        }
    }

    /// <summary>
    /// タイピング時のキーボード入力を有効化
    /// </summary>
    private void EnableKeyboardInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // キーボードの入力を受け取る
        keyboard.onTextInput += OnKeyboardInput;
    }

    /// <summary>
    /// タイピング時のキーボード入力を無効化
    /// </summary>
    private void DisableKeyboardInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // キーボード入力の受取り解除
        keyboard.onTextInput -= OnKeyboardInput;
    }
}
