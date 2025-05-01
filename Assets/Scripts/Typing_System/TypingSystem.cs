using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TypingSystem : MonoBehaviour
{
    private TypingJudger typingJudge;
    private List<TypingQuest> questList;
    private int questIndex = 0;

    [SerializeField]
    private TextAsset csvFile;
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
        questList = LoadCSV(csvFile);
        Init();
    }

    private void Init()
    {
        if (questIndex >= questList.Count)
        {
            DisableKeyboardInput();
            japaneseText.text = "Game Clear";
            romaText.text = "Game Clear";
            inputText.text = null;
            return;
        }

        TypingQuest currentQuest = questList[questIndex];
        japaneseText.text = currentQuest.japanese;
        romaText.text = currentQuest.roma;
        inputText.text = null;

        typingJudge = new TypingJudger(currentQuest.roma);
        questIndex++;
    }

    private void OnKeyboardInput(char ch)
    {
        switch (typingJudge.JudgeWord(ch))
        {
            case TypingState.Hit:
                inputText.text += ch;
                Debug.Log("Hit");
                break;
            case TypingState.Miss:
                Debug.Log("Miss");
                break;
            case TypingState.Clear:
                Debug.Log("Clear");
                Init();
                break;
            default:
                Debug.Log("Error");
                break;
        }
    }

    private List<TypingQuest> LoadCSV(TextAsset csvFile)
    {
        StringReader reader = new StringReader(csvFile.text);
        var questions = new List<TypingQuest>();

        bool isHeader = true;

        while (reader.Peek() > -1)
        {
            var line = reader.ReadLine();
            if (isHeader)
            {
                isHeader = false; // 最初の1行はヘッダーとしてスキップ
                continue;
            }

            var values = line.Split(',');

            if (values.Length >= 2)
            {
                questions.Add(new TypingQuest(values[0], values[1]));
            }
        }

        reader.Close();
        return questions;
    }

    private void EnableKeyboardInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // キーボードの入力を受け取る
        keyboard.onTextInput += OnKeyboardInput;
    }

    private void DisableKeyboardInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // キーボード入力の受取り解除
        keyboard.onTextInput -= OnKeyboardInput;
    }
}

class TypingQuest
{
    public string japanese { get; private set; }
    public string roma { get; private set; }

    public TypingQuest(string japanese, string roma)
    {
        this.japanese = japanese;
        this.roma = roma;
    }
}
