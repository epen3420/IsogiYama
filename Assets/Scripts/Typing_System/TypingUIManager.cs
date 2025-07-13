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

    private const string TYPED_TEXT_COLOR = "#BA3E06";

    private void Start()
    {
        progressManager.incorrectTyping += UpdateIncorrectTypeCount;
        progressManager.endTypingScene += End;
        progressManager.onUpdateAllTexts += UpdateDisplayTexts;

        progressManager.onResetUIText += ResetText;
        progressManager.onHideTextWindow += HideTextWindow;

        typoCountText.text = "0回";
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

    public void UpdateDisplayTexts(string fullJapanese, int typedJapaneseCount, string fullRomaji, int typedRomajiCount)
    {
        // 日本語テキストの更新: typedJapaneseCount は完全にタイプされたひらがなの文字数
        string coloredJapaneseText = GetColoredText(fullJapanese, typedJapaneseCount);
        japaneseText.SetText(coloredJapaneseText);

        // ローマ字テキストの更新
        string coloredRomajiText = GetColoredText(fullRomaji, typedRomajiCount);
        romaText.SetText(coloredRomajiText);
    }

    private string GetColoredText(string fullText, int typedCount)
    {
        int count = 0;
        int index = 0;

        while (index < fullText.Length && count < typedCount)
        {
            if (fullText[index] == '\\' && index + 1 < fullText.Length && fullText[index + 1] == 'n')
            {
                index += 2;
            }
            else if (fullText[index] == '\r' || fullText[index] == '\n')
            {
                index++;
            }
            else
            {
                count++;
                index++;
            }
        }

        string coloredPart = fullText.Substring(0, index);
        string rest = fullText.Substring(index);
        return $"<color={TYPED_TEXT_COLOR}>{coloredPart}</color>{rest}";
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
            progressManager.onUpdateAllTexts -= UpdateDisplayTexts;

            progressManager.onResetUIText -= ResetText;
            progressManager.onHideTextWindow -= HideTextWindow;
        }
    }
}
