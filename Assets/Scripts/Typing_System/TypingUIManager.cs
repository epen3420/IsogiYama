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

    private int typedCharCount = 0;
    private string originalRomaText;

    private void Start()
    {
        progressManager.correctTyping += UpdateInputText;
        progressManager.incorrectTyping += UpdateIncorrectTypeCount;
        progressManager.endCurrentQuest += SetUIText;
        progressManager.endTypingScene += End;

        typoCountText.text = "0回";
    }

    public void SetUIText(string japanese, string roma)
    {
        // inputText.maxVisibleCharacters = 0;
        typedCharCount = 0;

        japaneseText.text = japanese;
        romaText.text = roma;
        originalRomaText = roma;
        // inputText.text = roma;
    }

    public void ResetText()
    {
        japaneseText.text = "";
        romaText.text = "";
        // inputText.text = "";
    }

    public void HideTextWindow()
    {
        textWindow.SetActive(false);
    }

    public void UpdateInputText()
    {
        // inputText.maxVisibleCharacters++;
        typedCharCount++;

        if (romaText.text == null) return;

        string coloredText = "";

        // 入力済みの部分に色を付ける
        if (typedCharCount > 0)
        {
            coloredText += $"<color=#BA3E06>{originalRomaText.Substring(0, typedCharCount)}</color>";
        }
        // 未入力の部分を追加
        if (typedCharCount < originalRomaText.Length)
        {
            coloredText += originalRomaText.Substring(typedCharCount);
        }

        Debug.Log(typedCharCount);
        romaText.SetText(coloredText);
    }

    public void UpdateIncorrectTypeCount(int count)
    {
        typoCountText.SetText($"{count}回");
    }

    private void Update()
    {
        timerText.text = $"{timer.GetTime():F1}";
        // typoCountText.SetText($"回");
    }

    private void End(bool isGameOver)
    {
        ResetText();
        if (isGameOver)
        {
            HideTextWindow();
        }
    }
}
