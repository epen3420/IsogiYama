using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class AdjustCanvasForAspectRatio : MonoBehaviour
{
    private RectTransform canvasRectTransform;

    [SerializeField]
    private float targetAspect = 4f / 3f; // 固定したいアスペクト比

    void Awake()
    {
        canvasRectTransform = GetComponent<RectTransform>();
        AdjustCanvas();
    }

    void AdjustCanvas()
    {
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // 横に黒帯
            // Canvasの高さだけを調整
            canvasRectTransform.anchorMin = new Vector2(0, (1.0f - scaleHeight) / 2.0f);
            canvasRectTransform.anchorMax = new Vector2(1, 1.0f - (1.0f - scaleHeight) / 2.0f);
            canvasRectTransform.offsetMin = Vector2.zero;
            canvasRectTransform.offsetMax = Vector2.zero;
        }
        else
        {
            // 縦に黒帯
            // Canvasの幅だけを調整
            float scaleWidth = 1.0f / scaleHeight;
            canvasRectTransform.anchorMin = new Vector2((1.0f - scaleWidth) / 2.0f, 0);
            canvasRectTransform.anchorMax = new Vector2(1.0f - (1.0f - scaleWidth) / 2.0f, 1);
            canvasRectTransform.offsetMin = Vector2.zero;
            canvasRectTransform.offsetMax = Vector2.zero;
        }
    }
}
