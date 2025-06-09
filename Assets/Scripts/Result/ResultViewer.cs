using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// TypingResult のデータを取得し、TextMeshPro に書式付きで反映するコンポーネント
/// </summary>
public class ResultDisplay : MonoBehaviour
{
    [Header("表示先の TextMeshProUGUI")]
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text adviceText;
    [SerializeField] private TMP_Text ed3AdviceText;

    [Header("エンディングごとのスチル画像リスト")]
    [SerializeField] private List<Sprite> happyEndSprites;   // ハッピーエンド用のスプライト
    [SerializeField] private List<Sprite> badEndSprites;     // バッドエンド用のスプライト

    [SerializeField] private bool useTestData = false; // テストデータを使用するかどうか

    private VFXController vfxController;
    private List<Sprite> currentSprites;
    private int spriteIndex = 0;
    private Dictionary<string,  (string description, bool isUnlocked, bool isSpecial)> allEndings = new Dictionary<string, (string description, bool isUnlocked, bool isSpecial)>();
    private const string THEME_COLOR = "#FB570F"; // テーマカラーの定義

    private void Start()
    {
        vfxController = InstanceRegister.Get<VFXController>();
        vfxController.FadeOutCanvasAsync(0.8f).Forget();

        // 初期化
        adviceText.enabled = false;
        ed3AdviceText.enabled = false;
        allEndings = ResultHolder.instance.GetAllEndings();

        var typingResult = GetResultData();
        if (typingResult == null)
        {
            Debug.LogWarning("ResultHolder にデータが入っていません。");
            return;
        }

        // branchName を判定して currentStills をセット
        string branchName = typingResult.EndingBranchCondition.nextStep.CsvFile.name;
        bool isEd3 = branchName == "ed3_perf";
        currentSprites = isEd3 ? happyEndSprites : badEndSprites;

        // 最初の背景を表示
        ShowCurrentSprite(0, 0f);
        resultText.enabled = true;

        // 結果文字列の生成と反映
        BuildResultString(resultText, typingResult);

        if(isEd3)
        {
            BuildAdviceString(ed3AdviceText, typingResult, true);
        }
        else
        {
            BuildAdviceString(adviceText, typingResult, false);
            adviceText.enabled = true;
        }
    }

    private void Update()
    {
        bool clicked = Mouse.current.leftButton.wasPressedThisFrame;
        bool spaced = Keyboard.current.spaceKey.wasPressedThisFrame;

        if (clicked || spaced)
        {
            spriteIndex++;

            if (spriteIndex < currentSprites.Count)
            {
                // 次のスチルを表示
                if (spriteIndex == 1)
                {
                    resultText.enabled = false;
                    ed3AdviceText.enabled = true;
                }
                ShowCurrentSprite(spriteIndex, 1f);
            }
            else
            {
                // リストを使い切ったらシーン遷移
                ResultHolder.instance.ClearResult();

                // おそらくGameFlowManagerの管轄にした方がいい気がする
                // var sceneLoader = InstanceRegister.Get<SceneLoader>();
                // sceneLoader.LoadNextScene("TitleScene");
                GameFlowManager.instance.GoToNextScene();
            }
        }
    }

    /// <summary>
    /// TypingResult から表示用の結果文字列を組み立てて返す
    /// </summary>
    private void BuildResultString(TMP_Text resultText,TypingResult typingResult)
    {
        var sb = new StringBuilder();

        // 各パートのクリアタイムとミス数
        int partCount = typingResult.PartCount;
        for (int i = 0; i < partCount; i++)
        {
            (float partTime, int missCount) = typingResult.GetPartInfo(i);
            sb.AppendLine($"{i + 1}回目    {partTime:F1}秒 / {missCount}ミス");
            sb.AppendLine();
        }

        sb.AppendLine();

        // 合計タイム・ミス数
        float totalTime = typingResult.ClearTime;
        int totalMiss = typingResult.TotalIncorrectTypes;

        sb.AppendLine($"<size=70><color=#FB570F>合計</color>    {totalTime:F1}秒 / {totalMiss}ミス</size>");
        sb.AppendLine();

        // タイピング速度
        float speed = typingResult.GetTypingWPS();
        sb.AppendLine($"{speed:F1}文字 / 秒");
        sb.AppendLine();

        // 苦手な文字
        if (totalMiss != 0)
        {
            var worstKeys = typingResult.GetWorstMistypedKeys(3);
            var upperKeys = worstKeys.Select(c => char.ToUpper(c));
            string joinedKeys = string.Join(", ", upperKeys.Select(c => c.ToString()));
            sb.Append($"<color=#FB570F>苦</color>手な文字  {joinedKeys}");
        }
        else
        {
            sb.Append("<color=#FB570F>苦</color>手な文字  なし");
        }

        resultText.SetText(sb.ToString());
    }

    private void BuildAdviceString(TMP_Text endingHintText, TypingResult typingResult, bool showAllEndings = false)
    {
        StringBuilder stringBuilder = new StringBuilder();
        double currentScore = typingResult.GetCurrentScore();
        stringBuilder.AppendLine("Hint\n\n");

        float improveTime = 0;
        int improveMissType = 0;
        if (currentScore >= 0.8)
        {
            // 0.8以上
            // Ed3の場合はEd2もしくはEd1に行けるようにアドバイスする

            if (ResultHolder.instance.IsEndingUnlocked("ED2"))
            {
                // Ed2がアンロックされている場合は、Ed1に行けるようにアドバイス
                improveTime = (float)typingResult.GetRequiredWForTargetScore(0.5);
                improveMissType = (int)typingResult.GetRequiredEForTargetScore(0.5);
                Debug.Log("ED3 For Go To ED1 Advice");
            }
            else
            {
                // Ed2がアンロックされていない場合は、Ed2行けるようにアドバイス
                improveTime = (float)typingResult.GetRequiredWForTargetScore(0.8);
                improveMissType = (int)typingResult.GetRequiredEForTargetScore(0.8);
                Debug.Log("ED3 For Go To ED2 Advice");
            }
        }
        else if (currentScore >= 0.5)
        {
            // 0.5以上0.8未満
            // Ed2の場合はEd3に行けるようにアドバイスする

            improveTime = (float)typingResult.GetRequiredWForTargetScore(0.8);
            improveMissType = (int)typingResult.GetRequiredEForTargetScore(0.8);
            Debug.Log("ED2 For Go To ED3 Advice");
        }
        else
        {
            // 0.5未満
            // Ed1の場合はEd2(もしくはEd3)に行けるようにアドバイスする

            improveTime = (float)typingResult.GetRequiredWForTargetScore(0.5);
            improveMissType = (int)typingResult.GetRequiredEForTargetScore(0.5);
            Debug.Log("ED1 For Go To ED2 Advice");
        }

        if (improveMissType > 0)
        {
            stringBuilder.AppendLine($"ミスタイプを<color={THEME_COLOR}>{improveMissType}</color>回減らすと...");
        }
        else
        {
            stringBuilder.AppendLine($"ミスタイプを<color={THEME_COLOR}>{improveMissType * -1}</color>回増やすと...");
        }

        if (improveTime > 0)
        {
            stringBuilder.AppendLine($"クリア時間をあと<color={THEME_COLOR}>{improveTime}</color>秒早くすると...");
        }
        else
        {
            stringBuilder.AppendLine($"クリア時間をあと<color={THEME_COLOR}>{improveTime * -1}</color>秒遅くすると...");
        }

        stringBuilder.AppendLine("新しいエンディングにいけるかもしれない。");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine();

        if (showAllEndings)
        {
            stringBuilder.AppendLine("エンディング一覧");
            stringBuilder.AppendLine("====================");

            foreach (var entry in allEndings)
            {
                string endingName = entry.Key;
                string description = entry.Value.description;
                bool isUnlocked = entry.Value.isUnlocked;
                bool isSpecial = entry.Value.isSpecial;

                if (isUnlocked)
                {
                    if (isSpecial)
                    {
                        stringBuilder.AppendLine($"<color=red>・{endingName} <size=20>{description}</size></color>");
                    }
                    else
                    {
                        stringBuilder.AppendLine($"・{endingName} <size=20>{description}</size>");
                    }
                }
                else
                {
                    if (isSpecial)
                    {
                        stringBuilder.AppendLine("<color=red>・???</color>");
                    }
                    else
                    {
                        stringBuilder.AppendLine("・???");
                    }
                }
                stringBuilder.AppendLine();
            }
        }
        

        if (endingHintText != null)
        {
            endingHintText.SetText(stringBuilder.ToString());
        }
        else
        {
            Debug.LogError("TMP_Text component is not assigned to endingHintText.");
        }
    }

    /// <summary>
    /// 指定したインデックスのスプライトを VFXController に渡して背景を変更します。
    /// </summary>
    private void ShowCurrentSprite(int index, float fadeDuration)
    {
        Sprite sprite = currentSprites[index];
        // スプライト名を ChangeBackgroundAsync の引数に
        vfxController.ChangeBackgroundAsync(sprite.name, fadeDuration).Forget();
    }

    private TypingResult GetTestData()
    {
        TypingResult testResult = new TypingResult();

        int partCount = 3;
        for (int i = 0; i < partCount; i++)
        {
            int correctCount = Random.Range(30, 300);

            int missCount = Random.Range(0, 1);

            float partTime = Random.Range(15.0f, 80.0f);

            testResult.AddPartResult(correctCount, missCount, partTime);

            for (int j = 0; j < missCount; j++)
            {
                char randomKey = (char)Random.Range((int)'a', (int)'z' + 1);
                testResult.AddMistypedKey(randomKey);
            }
        }

        return testResult;
    }

    private TypingResult GetResultData()
    {
        if (useTestData)
        {
            // テストデータを使用する場合
            return GetTestData();
        }

        // ResultHolder から TypingResult を取得
        var result = ResultHolder.instance.GetResult();

        if (result == null)
        {
            Debug.LogWarning("ResultHolder に TypingResult が設定されていません。");
            return GetTestData();
        }

        return result;
    }
}
