using UnityEngine;
using TMPro;

public class VersionDisplay : MonoBehaviour
{
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
            Debug.LogWarning("Version Text (TextMeshProUGUI) is not assigned. Displaying in Debug.Log: " + displayString);
            Debug.Log(displayString);
        }
    }

    string GetPlatformString()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return "Windows";
            case RuntimePlatform.WebGLPlayer:
                return "WebGL";
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXEditor:
                return "macOS";
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "iOS";
            case RuntimePlatform.LinuxPlayer:
            case RuntimePlatform.LinuxEditor:
                return "linux";
            default:
                return "unknown";
        }
    }
}