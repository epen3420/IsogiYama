using TMPro;
using UnityEngine;

public class TypingUIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text japaneseText;
    [SerializeField]
    private TMP_Text romaText;

    [SerializeField]
    private StopwatchTimer timer;
    [SerializeField]
    private TMP_Text timerText;
    [SerializeField]
    private TMP_Text typoCountText;

    [SerializeField]
    private GameObject textWindow;

    [SerializeField]
    private TypingProgressManager progressManager;

    private string currentOriginalRomaText; // ローマ字が変更された場合に備えて、現在の表示対象を保持

    private void Start()
    {
        progressManager.incorrectTyping += UpdateIncorrectTypeCount;
        progressManager.endTypingScene += End;

        progressManager.onUpdateJapaneseText += SetJapaneseText;
        progressManager.onSetInitialRomajiText += SetRomajiText;
        progressManager.onUpdateRomajiText += UpdateInputText;
        progressManager.onResetUIText += ResetText;
        progressManager.onHideTextWindow += HideTextWindow;

        typoCountText.text = "0回";
    }

    // 日本語テキストを個別に設定するメソッド
    public void SetJapaneseText(string japanese)
    {
        japaneseText.text = japanese;
    }

    // ローマ字テキストと現在の入力済み長さを引数で受け取る (TypingJudderから直接呼び出されることを想定し、RomaTextを更新する)
    // このメソッドはprogressManager.onSetInitialRomajiTextイベントのハンドラとしても機能
    public void SetRomajiText(string newRomaji)
    {
        currentOriginalRomaText = newRomaji;
        UpdateInputText(newRomaji, 0); // 初期表示として、まだ何も入力されていない状態に設定
    }

    public void ResetText()
    {
        japaneseText.text = "";
        romaText.text = "";
    }

    public void HideTextWindow()
    {
        textWindow.SetActive(false);
    }

    public void UpdateInputText(string fullRomaji, int typedCharCount)
    {
        currentOriginalRomaText = fullRomaji;

        if (currentOriginalRomaText == null) return;

        string coloredText = "";

        // 入力済みの部分に色を付ける
        // typedCharCountがcurrentOriginalRomaText.Lengthを超える可能性があるので、Minを使用
        int charsToColor = Mathf.Min(typedCharCount, currentOriginalRomaText.Length);
        if (charsToColor > 0)
        {
            coloredText += $"<color=#BA3E06>{currentOriginalRomaText.Substring(0, charsToColor)}</color>";
        }
        // 未入力の部分を追加
        if (typedCharCount < currentOriginalRomaText.Length)
        {
            coloredText += currentOriginalRomaText.Substring(charsToColor);
        }

        // Debug.Log($"Typed: {typedCharCount}, Text: {coloredText}");
        romaText.SetText(coloredText);
    }

    public void UpdateIncorrectTypeCount(int count)
    {
        typoCountText.SetText($"{count}回");
    }

    private void Update()
    {
        timerText.text = $"{timer.GetTime():F1}";
    }

    private void End(bool isGameOver)
    {
        ResetText();
        if (isGameOver)
        {
            HideTextWindow();
        }
    }

    private void OnDestroy()
    {
        if (progressManager != null)
        {
            progressManager.incorrectTyping -= UpdateIncorrectTypeCount;
            progressManager.endTypingScene -= End;

            progressManager.onUpdateJapaneseText -= SetJapaneseText;
            progressManager.onSetInitialRomajiText -= SetRomajiText;
            progressManager.onUpdateRomajiText -= UpdateInputText;
            progressManager.onResetUIText -= ResetText;
            progressManager.onHideTextWindow -= HideTextWindow;
        }
    }
}
