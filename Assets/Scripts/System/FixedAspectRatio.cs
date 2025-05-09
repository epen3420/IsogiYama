using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FixedAspectRatio : MonoBehaviour
{
    public float targetAspect = 4f / 3f; // 固定したいアスペクト比（ここでは4:3）

    void Start()
    {
        Camera cam = GetComponent<Camera>();

        // 現在の画面のアスペクト比
        float windowAspect = (float)Screen.width / (float)Screen.height;

        // アスペクト比の比率
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // 横に黒帯（レターボックス）
            Rect rect = cam.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            cam.rect = rect;
        }
        else
        {
            // 縦に黒帯（ピラーボックス）
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = cam.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            cam.rect = rect;
        }
    }
}
