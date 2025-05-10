using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using IsogiYama.System;
using SoundSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class TypingProgressManager : SceneSingleton<TypingProgressManager>
{
    // インスタンスの保持
    private GameFlowManager gameFlowManager;
    private SoundPlayer soundPlayer;
    private TypingJudder typingJudger;
    private TypingBGScheduler typingBGScheduler;

    // 日本語とローマ字の対応リスト
    private List<JapaneseRomaPair> questDatas;
    [System.Serializable]
    private struct JapaneseRomaPair
    {
        public string Japanese;
        public string Roma;

        public JapaneseRomaPair(string japanese, string roma)
        {
            Japanese = japanese;
            Roma = roma;
        }
    }
    private int questIndex = 0;

    private bool hasStartedTimer = false;

    [SerializeField]
    private TypingUIManager typingUIManager;
    [SerializeField]
    private StopwatchTimer timer;


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

    /// <summary>
    /// インスタンスの取得⇒CSVデータの取得⇒画面とTypingJudderの準備
    /// ⇒フェードアウト⇒キーボード入力の受付を開始
    /// </summary>
    private async void Start()
    {
        // インスタンスの生成と参照
        gameFlowManager = GameFlowManager.instance;
        soundPlayer = SoundPlayer.instance;

        var isInitComplete = InitTypingData();

        if (!isInitComplete) return;

        // 初期化処理をしてからフェードアウトし、タイピングのスタート
        NextQuest();
        await typingBGScheduler.FadeOut();
        typingBGScheduler.OnGameOver += GameOver;
        EnableKeyboardInput();
    }

    private bool InitTypingData()
    {
        // CSVファイルの取得
        var csvFile = gameFlowManager.GetCurrentCSV();
        if (csvFile == null)
        {
            Debug.LogError("CSV file is null in typing scene. Please check GameFlowDatabase.");
            return false;
        }

        // CSVファイルからデータを抽出
        var csvLoader = new CSVLoader();
        var csvData = csvLoader.LoadCSV<TypingQuestType>(csvFile);
        if (csvData == null)
        {
            Debug.LogError("Failed to load CSV data.");
            return false;
        }

        StoreCSVDataToList(csvData);
        typingBGScheduler = new TypingBGScheduler(csvData.Rows[0], timer);
        return true;
    }

    private void NextQuest()
    {
        if (questIndex >= questDatas.Count)
        {
            End();
            return;
        }
        typingUIManager.ResetText();

        var currentQuestData = questDatas[questIndex++];
        typingJudger = new TypingJudder(currentQuestData.Roma);

        typingUIManager.SetUIText(currentQuestData.Japanese, currentQuestData.Roma);
    }

    private void End()
    {
        timer.StopTimer();
        DisableKeyboardInput();

        var clearTime = timer.GetTime();
        Debug.Log($"Clear Time: {clearTime}");

        gameFlowManager.SetClearTime(clearTime);

        typingBGScheduler.FadeIn();

        timer.ResetTimer();
        typingUIManager.ResetText();

        gameFlowManager.GoToNextScene();
        return;
    }

    private void GameOver()
    {
        typingBGScheduler.FadeIn();

        timer.ResetTimer();
        typingUIManager.ResetText();

        gameFlowManager.GameOver().Forget();
    }

    /// <summary>
    /// タイピングのクエストデータをCSVファイルから作成
    /// </summary>
    /// <param name="csvData"></param>
    private void StoreCSVDataToList(CsvData<TypingQuestType> csvData)
    {
        questDatas = new List<JapaneseRomaPair>();
        // タイピングのクエストデータをリストに格納
        foreach (var row in csvData.Rows)
        {
            var japanese = row.Get<string>(TypingQuestType.japanese);
            var roma = row.Get<string>(TypingQuestType.roma);

            questDatas.Add(new JapaneseRomaPair(japanese, roma));
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
                if (!hasStartedTimer)
                {
                    hasStartedTimer = true;
                    timer.StartTimer();
                }
                typingUIManager.UpdateInputText();
                soundPlayer.PlaySe("TypeHit");

                Debug.Log($"{typedChar}: Hit");
                break;

            case TypingState.Miss:
                Debug.Log($"{typedChar}: Miss");
                soundPlayer.PlaySe("TypeMiss");
                break;

            case TypingState.Clear:
                typingUIManager.UpdateInputText();
                soundPlayer.PlaySe("TypeHit");

                Debug.Log($"{typedChar}: Clear");

                NextQuest();
                break;

            default:
                Debug.Log("Error");
                break;
        }
    }
}
