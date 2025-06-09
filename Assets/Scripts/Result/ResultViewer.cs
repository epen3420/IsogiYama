using Cysharp.Threading.Tasks;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem; // 追加：InputSystem対応

/// <summary>
/// TypingResult のデータを取得し、TextMeshPro に書式付きで反映するコンポーネント
/// </summary>
public class ResultDisplay : MonoBehaviour
{
    [Header("表示先の TextMeshProUGUI")]
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text adviceText;
    [SerializeField] private bool useTestData = false; // テストデータを使用するかどうか

    private VFXController vfxController;

    // Ed3用フラグ
    private bool isEd3 = false;
    private bool hasTransitioned = false;

    private void Start()
    {
        vfxController = InstanceRegister.Get<VFXController>();

        // adviceText はデフォルト非表示に
        adviceText.enabled = false;

        var typingResult = GetResultData();
        if (typingResult == null)
        {
            Debug.LogWarning("ResultHolder にデータが入っていません。");
            return;
        }

        // 将来的にEnumで分岐したい、文字列分岐は望ましくない
        string branchName;
        try
        {
            branchName = typingResult.EndingBranchCondition.nextStep.CsvFile.name;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"ブランチ名の取得に失敗: {ex.Message}");
            branchName = "ed1_perf";
            return;
        }

        if (branchName == "ed3_perf")
        {
            isEd3 = true;
            vfxController.ChangeBackgroundAsync("ResultEd3", 0).Forget();
            resultText.enabled = true;
            adviceText.enabled = false;
        }
        else if (branchName == "ed1_perf" || branchName == "ed2_perf")
        {
            isEd3 = false;
            vfxController.ChangeBackgroundAsync("ResultBad", 0).Forget();
            resultText.enabled = true;
            adviceText.enabled = true;
            adviceText.text =
                "Hint\n\n" +
                "ミスタイプをNull回減らそう...\n\n" +
                "クリア時間をあとNull秒早くしよう...";
            // ※ Null の部分の処理は後で実装
        }

        // StringBuilder で文字列を組み立て
        var sb = new StringBuilder();

        int partCount = typingResult.PartCount;
        for (int i = 0; i < partCount; i++)
        {
            // 各パートのクリアタイムとミス数を取得
            (float partTime, int missCount) = typingResult.GetPartInfo(i);

            sb.AppendLine($"{i + 1}回目    {partTime:F1}秒 / {missCount}ミス");
            sb.AppendLine();
        }

        sb.AppendLine();

        // 合計行を表示
        float totalTime = typingResult.ClearTime;
        int totalMiss = 0;
        // PartCount が分かるので、合計ミス数を足していく
        for (int i = 0; i < partCount; i++)
        {
            totalMiss += typingResult.GetPartInfo(i).missCount;
        }

        sb.AppendLine($"<size=70><color=#FB570F>合計</color>    {totalTime:F1}秒 / {totalMiss}ミス</size>");
        sb.AppendLine();

        // タイピング速度
        float speed = typingResult.GetTypingSpeed(); // 文字/秒
        sb.AppendLine($"{speed:F1}文字 / 秒");
        sb.AppendLine();

        // 苦手な文字
        var worstKeys = typingResult.GetWorstMistypedKeys(3);
        if (worstKeys != null && worstKeys.Length > 0)
        {
            // ここで char.ToUpper を使って大文字に変換する
            var upperKeys = worstKeys.Select(c => char.ToUpper(c));
            string joinedKeys = string.Join(", ", upperKeys.Select(c => c.ToString()));
            sb.Append($"<color=#FB570F>苦</color>手な文字  {joinedKeys}");
        }
        else
        {
            sb.Append("<color=#FB570F>苦</color>手な文字  なし");
        }

        // 完成した文字列を TextMeshPro にセット
        resultText.text = sb.ToString();
    }

    private void Update()
    {
        // Ed3用のクリック or スペースキーで背景遷移
        if (isEd3 && !hasTransitioned)
        {
            bool clicked = Mouse.current.leftButton.wasPressedThisFrame;
            bool spaced = Keyboard.current.spaceKey.wasPressedThisFrame;
            if (clicked || spaced)
            {
                vfxController.ChangeBackgroundAsync("Ed3_Phone", 0.5f).Forget();
                // resultText を非表示
                resultText.enabled = false;
                hasTransitioned = true;
            }
        }
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
