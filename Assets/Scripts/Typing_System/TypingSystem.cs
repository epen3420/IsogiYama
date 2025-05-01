using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TypingSystem : MonoBehaviour
{
    private TypingJudger typingJudge;
    private int questIndex = 0;

    [SerializeField]
    private List<string> questList = new List<string>();
    [SerializeField]
    private TMP_Text questionText;
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
        Init();
    }

    private void Init()
    {
        if (questIndex >= questList.Count)
        {
            DisableKeyboardInput();
            questionText.text = "Game Clear";
            inputText.text = null;
            return;
        }

        string currentString = questList[questIndex];
        questionText.text = currentString;
        inputText.text = null;

        typingJudge = new TypingJudger(currentString);
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
