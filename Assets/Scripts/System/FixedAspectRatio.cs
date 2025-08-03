using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteAlways]
public class FixedAspectRatio : MonoBehaviour
{
    // 固定したいアスペクト比
    [SerializeField]
    private Vector2 targetAspectVector = new Vector2(4, 3);

    private Camera targetCamera;
    private float currentScreenWidth = 0;
    private float currentScreenHeight = 0;

    void Awake()
    {
        targetCamera = GetComponent<Camera>();
        if (targetCamera == null)
        {
            Debug.LogError("Camera component not found on the GameObject. Please attach this script to a GameObject with a Camera.");
            return;
        }

        // 初期設定をAwakeで一度だけ実行
        UpdateCameraAspect();
    }

    void Update()
    {
        // 画面サイズが変更された時のみアスペクト比を再計算
        if (currentScreenWidth != Screen.width || currentScreenHeight != Screen.height)
        {
            UpdateCameraAspect();
        }
    }

    private void UpdateCameraAspect()
    {
        currentScreenWidth = Screen.width;
        currentScreenHeight = Screen.height;

        // 現在の画面と目的のアスペクト比を計算
        float screenAspect = currentScreenWidth / currentScreenHeight;
        float targetAspect = targetAspectVector.x / targetAspectVector.y;
        float aspectScale = targetAspect / screenAspect;

        Rect viewportRect = new Rect(0, 0, 1, 1);

        if (aspectScale < 1.0f)
        {
            // 画面が目的のアスペクト比より縦長の場合（左右に黒帯）
            viewportRect.width = aspectScale;
            viewportRect.x = (1.0f - aspectScale) * 0.5f;
            viewportRect.height = 1.0f;
            viewportRect.y = 0;
        }
        else
        {
            // 画面が目的のアスペクト比より横長の場合（上下に黒帯）
            viewportRect.height = 1.0f / aspectScale;
            viewportRect.y = (1.0f - viewportRect.height) * 0.5f;
            viewportRect.width = 1.0f;
            viewportRect.x = 0;
        }

        targetCamera.rect = viewportRect;
    }
}
