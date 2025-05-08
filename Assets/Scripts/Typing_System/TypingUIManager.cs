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

    public void UpdateInputText()
    {
        inputText.maxVisibleCharacters++;
    }

    private void Update()
    {
        timerText.text = $"{timer.GetTime():F1}";
    }
}
