using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TypingSystem : MonoBehaviour
{
    private TypingJudger typingJudger; // タイピング判定用クラス
    private List<TypingQuest> questList; // 問題を格納するリスト
    private int questIndex = 0;

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
        /*実際のStartメソッドは以下
            questList = LoadCSV(GameFlowManager.instance.GetCurrentCSV());

            Init();
        */
        Invoke("Init", 1.0f);
    }

    private void Init()
    {
        // 以下の行は実際は消す
        questList = LoadCSV(GameFlowManager.instance.GetCurrentCSV());
        if (questIndex >= questList.Count)
        {
            DisableKeyboardInput();
            GameFlowManager.instance.GoToNextScene();

            japaneseText.text = "Game Clear";
            romaText.text = "Game Clear";
            inputText.text = null;

            return;
        }

        TypingQuest currentQuest = questList[questIndex++];

        japaneseText.text = currentQuest.japanese;
        romaText.text = currentQuest.roma;
        inputText.text = null;

        typingJudger = new TypingJudger(currentQuest.roma);
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
                inputText.text += typedChar;

                Debug.Log($"{typedChar}: Hit");
                break;

            case TypingState.Miss:
                Debug.Log($"{typedChar}: Miss");
                break;

            case TypingState.Clear:
                Debug.Log($"{typedChar}: Clear");

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
