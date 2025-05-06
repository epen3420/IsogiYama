using IsogiYama.System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TypingSystem : MonoBehaviour
{
    private GameFlowManager gameFlowManager;
    private TypingJudger typingJudger; // タイピング判定用クラス
    private CsvData<TypingQuestType> questList; // 問題を格納するリスト
    private int questIndex = 0;

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

        var csvLoader = new CSVLoader();
        var csvFile = gameFlowManager.GetCurrentCSV();
        questList = csvLoader.LoadCSV<TypingQuestType>(csvFile);

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

                Debug.Log($"{typedChar}: Hit");
                break;

            case TypingState.Miss:
                Debug.Log($"{typedChar}: Miss");
                break;

            case TypingState.Clear:
                inputText.maxVisibleCharacters++;

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
