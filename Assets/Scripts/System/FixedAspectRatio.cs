using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FixedAspectRatio : MonoBehaviour
{
    // 固定したいアスペクト比
    [SerializeField]
    private float targetAspect = 4f / 3f;

    private const float FULL_SCREEN = 1.0f;

    void Awake()
    {
        Camera cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("Camera component not found on the GameObject.");
            return;
        }

        // 現在の画面のアスペクト比
        float windowAspect = (float)Screen.width / Screen.height;

        // 目標アスペクト比に対する現在の画面アスペクト比の比率
        float aspectScale = windowAspect / targetAspect;

        Rect rect = cam.rect;

        if (aspectScale < FULL_SCREEN)
        {
            // 画面が横長すぎるとき縦に黒帯
            rect.y = (FULL_SCREEN - aspectScale) / 2.0f;
            rect.height = aspectScale;
            rect.x = 0;
            rect.width = FULL_SCREEN;
        }
        else
        {
            // 画面が縦長すぎるとき横に黒帯
            float scaleWidth = FULL_SCREEN / aspectScale;
            rect.x = (FULL_SCREEN - scaleWidth) / 2.0f;
            rect.width = scaleWidth;
            rect.y = 0;
            rect.height = FULL_SCREEN;
        }

        cam.rect = rect;
    }
}
