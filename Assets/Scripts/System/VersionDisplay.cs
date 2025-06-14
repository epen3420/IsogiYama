using UnityEngine;
using TMPro; // TextMeshProを使用する場合に必要です

public class VersionDisplay : MonoBehaviour
{
    // UnityエディタからTextMeshProUGUIコンポーネントをここにドラッグ＆ドロップしてください
    public TextMeshProUGUI versionText;

    void Start()
    {
        DisplayVersionInfo();
    }

    void DisplayVersionInfo()
    {
        string platform = GetPlatformString();

        string appVersion = Application.version;

        string displayString = $"{platform} v{appVersion}";

        if (versionText != null)
        {
            versionText.text = displayString;
        }
        else
        {
            // 割り当てられていない場合は、デバッグログに表示します
            Debug.LogWarning("Version Text (TextMeshProUGUI) is not assigned. Displaying in Debug.Log: " + displayString);
            Debug.Log(displayString);
        }
    }

    // 各プラットフォームに対応する短い文字列を返します
    string GetPlatformString()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return "win";
            case RuntimePlatform.WebGLPlayer:
                return "webgl";
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor:
                return "mac";
            case RuntimePlatform.Android:
                return "android";
            case RuntimePlatform.IPhonePlayer:
                return "ios";
            case RuntimePlatform.LinuxPlayer:
            case RuntimePlatform.LinuxEditor:
                return "linux";
            default:
                return "unknown";
        }
    }
}