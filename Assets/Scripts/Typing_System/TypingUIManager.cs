using TMPro;
using UnityEngine;

public class TypingUIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text japaneseText;
    [SerializeField]
    private TMP_Text romaText;
    [SerializeField]
    private TMP_Text inputText;

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

    private void Start()
    {
        progressManager.correctTyping += UpdateInputText;
        progressManager.incorrectTyping += UpdateIncorrectTypeCount;
        progressManager.endCurrentQuest += SetUIText;
        progressManager.endTypingScene += End;
    }

    public void SetUIText(string japanese, string roma)
    {
        inputText.maxVisibleCharacters = 0;

        japaneseText.text = japanese;
        romaText.text = roma;
        inputText.text = roma;
    }

    public void ResetText()
    {
        japaneseText.text = "";
        romaText.text = "";
        inputText.text = "";
    }

    public void HideTextWindow()
    {
        textWindow.SetActive(false);
    }

    public void UpdateInputText()
    {
        inputText.maxVisibleCharacters++;
    }

    public void UpdateIncorrectTypeCount(int count)
    {
        typoCountText.SetText($"{count}回");
    }

    private void Update()
    {
        timerText.text = $"{timer.GetTime():F1}";
        typoCountText.SetText($"回");
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
