#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class CheatModeWindow : EditorWindow
{
    public static bool IsCheat = false;
    private static float timeScale = 1.0f;

    static CheatModeWindow()
    {
        // プレイモードの状態変更イベントを購読
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    [MenuItem("CheatMenu/ShowCheatToggle")]
    public static void ShowWindow()
    {
        GetWindow<CheatModeWindow>("Cheat Mode");
    }

    private void OnGUI()
    {
        GUILayout.Label("チートモード設定", EditorStyles.boldLabel);

        IsCheat = EditorGUILayout.Toggle("Is Cheat Mode", IsCheat);

        timeScale = EditorGUILayout.Slider("Change Time Scale", timeScale, 0.0f, 10.0f);
        if (Application.isPlaying)
        {
            Time.timeScale = timeScale;
        }
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        // プレイモードからエディットモードに戻ったとき
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            timeScale = 1.0f;
        }
    }
}
#endif
