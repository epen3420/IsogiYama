using System.Collections.Generic;
using System.IO;
using System.Linq;
using IsogiYama.System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TypingSystem : MonoBehaviour
{
    private TypingJudger typingJudger; // タイピング判定用クラス
    private CsvData<TypingQuest> questList; // 問題を格納するリスト
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
        var csvLoader = new CSVLoader();
        questList = csvLoader.LoadCSV<TypingQuest>(csvFile);

        Init();
    }

    private void Init()
    {
        inputText.maxVisibleCharacters = 0;
        if (questIndex >= questList.Rows.Count)
        {
            DisableKeyboardInput();

            japaneseText.text = "Game Clear";
            romaText.text = null;
            inputText.text = null;

            return;
        }

        var currentQuest = questList.Rows[questIndex++];
        var currentJapanese = currentQuest.Get<string>(TypingQuest.japanese);
        var currentRoma = currentQuest.Get<string>(TypingQuest.roma);

        japaneseText.text = currentJapanese;
        romaText.text = currentRoma;
        inputText.text = currentRoma;

        typingJudger = new TypingJudger(currentRoma);
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
                inputText.maxVisibleCharacters++;

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

public enum TypingQuest
{
    japanese,
    roma
}
