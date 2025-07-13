using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using IsogiYama.System;
using SoundSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class TypingProgressManager : MonoBehaviour
{
    // Eventの定義
    public event Action correctTyping;
    public event Action<int> incorrectTyping;
    public event Action<string, string> endCurrentQuest; // 日本語とローマ字を渡すイベント

    // 日本語とローマ字のUI更新をまとめて行うイベント
    public event Action<string, int, string, int> onUpdateAllTexts; // fullJapanese, typedJapaneseCount, fullRomaji, RomaTypedCount

    // UIリセットとテキストウィンドウ非表示のイベント
    public event Action onResetUIText; // 追加
    public event Action onHideTextWindow; // 追加

    public event Action<bool> endTypingScene;

    // インスタンスの保持
    private GameFlowManager gameFlowManager;
    private TypingResult typingResult;
    private SoundPlayer soundPlayer;
    private TypingJudder typingJudder;
    private TypingBGScheduler typingBGScheduler;
    [SerializeField] // TypingUIManagerへの参照を追加
    private TypingUIManager typingUIManager;

    // 日本語とローマ字の対応リスト
    private List<JapaneseRomaPair> questDatas;
    private JapaneseRomaPair currentQuestData; // 現在のクエストデータ

    [System.Serializable]
    private struct JapaneseRomaPair
    {
        public string Japanese;
        public string Input; // ここはローマ字文字列

        public JapaneseRomaPair(string japanese, string input)
        {
            Japanese = japanese;
            Input = input;
        }
    }
    private int questIndex = 0;

    private bool hasStartedTimer = false;
    private int correctTypeCount = 0;
    private int missTypeCount = 0;

    [Header("ゲームオーバーになる秒数")]
    [SerializeField]
    private float gameOverTime = 180.0f;
    [Header("ゲームオーバー画面の表示時間 (ms)")]
    [SerializeField]
    private int displayTimeOfGameOverScreen = 5000;
    [Header("ゲームオーバー時に出す画像の名前")]
    [SerializeField]
    private string gameOverImageName = "Blood";
    [Header("ゲームオーバーになるミスタイプ数")]
    [SerializeField]
    private int maxMissTypeCount = 10;

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
        typingResult = ResultHolder.instance.GetResult();

        var isInitComplete = InitTypingData();

        if (!isInitComplete) return;

        // 初期化処理をしてからタイピング画面にフェードインし、タイピングのスタート
        NextQuest(); // NextQuest内でUI初期化を呼び出す
        await typingBGScheduler.FadeOut();
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
        typingBGScheduler = new TypingBGScheduler
        (
            csvData.Rows[0],
            timer,
            endTypingScene,
            gameOverImageName,
            gameOverTime,
            displayTimeOfGameOverScreen
        );

        return true;
    }

    private void NextQuest()
    {
        if (questIndex >= questDatas.Count)
        {
            End();
            return;
        }
        currentQuestData = questDatas[questIndex++];

        typingJudder = new TypingJudder(currentQuestData.Input); // Inputはローマ字文字列

        // Debug.Log
        foreach (var segment in typingJudder.judgeList)
        {
            Debug.Log(segment.ToString());
        }

        // UIの初期表示を新しいUpdateDisplayTextsメソッドで直接行う
        onUpdateAllTexts?.Invoke(
            currentQuestData.Japanese,
            0,
            typingJudder.FullRomajiText,
            0
        );

        // endCurrentQuestイベントは、他のシステムに通知するために保持
        endCurrentQuest?.Invoke(currentQuestData.Japanese, typingJudder.FullRomajiText);
    }

    private void End(bool isGameOver = false)
    {
        timer.StopTimer();
        DisableKeyboardInput();

        var clearTime = timer.GetTime();
        Debug.Log($"This scene clear time: {clearTime}");

        typingResult.AddPartResult(correctTypeCount, missTypeCount, clearTime);

        timer.ResetTimer();

        endTypingScene?.Invoke(isGameOver);

        // UIManagerにUIリセットとウィンドウ非表示を通知
        onResetUIText?.Invoke();
        if (isGameOver)
        {
            onHideTextWindow?.Invoke();
        }

        ResultHolder.instance.SetResult(typingResult);
        gameFlowManager.GoToNextScene(isGameOver);
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
            var input = row.Get<string>(TypingQuestType.input);

            questDatas.Add(new JapaneseRomaPair(japanese, input));
        }
    }

    /// <summary>
    /// タイピングの入力を受け取り、判定を行う
    /// </summary>
    /// <param name="typedChar"></param>
    private void OnKeyboardInput(char typedChar)
    {
        if (typedChar == ' ') return;

        // TypingJudgerから現在の入力済み文字数を取得し、UIManagerに渡す
        TypingState state = typingJudder.JudgeChar(typedChar);

        switch (state)
        {
            case TypingState.Hit:
                if (!hasStartedTimer)
                {
                    hasStartedTimer = true;
                    timer.StartTimer();
                }
                correctTypeCount++;
                soundPlayer.PlaySe("TypeHit");

                onUpdateAllTexts?.Invoke(
                    currentQuestData.Japanese,
                    typingJudder.GetCombinedHiraganaLength(),
                    typingJudder.FullRomajiText,
                    typingJudder.GetCurrentInputLength()
                );
                Debug.Log(typingJudder.GetCombinedHiraganaLength());
                // Debug.Log($"{typedChar}: Hit");
                break;

            case TypingState.Miss:
                if (!hasStartedTimer) break;

                missTypeCount++;
                typingResult.AddMistypedKey(typedChar);
                incorrectTyping?.Invoke(missTypeCount);

                // Debug.Log($"{typedChar}: Miss");
                soundPlayer.PlaySe("TypeMiss");

                break;

            case TypingState.Clear:
                correctTypeCount++;

                soundPlayer.PlaySe("TypeHit");
                onUpdateAllTexts?.Invoke(
                    currentQuestData.Japanese,
                    typingJudder.GetCombinedHiraganaLength(), // クリア時は全ての色付けが完了
                    typingJudder.FullRomajiText,
                    typingJudder.GetCurrentInputLength() // クリア時は全ての色付けが完了
                );

                Debug.Log($"{typedChar}: Clear");

                NextQuest();
                break;

            default:
                Debug.Log("Error");
                break;
        }
    }

    private void OnDestroy()
    {
        DisableKeyboardInput();
        if (onUpdateAllTexts != null && typingUIManager != null)
        {
            onUpdateAllTexts -= typingUIManager.UpdateDisplayTexts;
        }
        if (onResetUIText != null && typingUIManager != null)
        {
            onResetUIText -= typingUIManager.ResetText;
        }
        if (onHideTextWindow != null && typingUIManager != null)
        {
            onHideTextWindow -= typingUIManager.HideTextWindow;
        }
    }
}
