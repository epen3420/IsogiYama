#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class CheatModeWindow : EditorWindow
{
    public static bool IsCheat = false;

    [MenuItem("CheatMenu/ShowCheatToggle")]
    public static void ShowWindow()
    {
        GetWindow<CheatModeWindow>("Cheat Mode");
    }

    private void OnGUI()
    {
        GUILayout.Label("チートモード設定", EditorStyles.boldLabel);

        IsCheat = EditorGUILayout.Toggle("Is Cheat Mode", IsCheat);
    }
}
#endif
