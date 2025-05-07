using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PressAnyKeyButton : MonoBehaviour
{
    private bool isPressed = false;

    [SerializeField]
    private float minColorAlpha = 0.5f;
    [SerializeField]
    private float maxColorAlpha = 1.0f;
    [SerializeField]
    private float flashDuration = 0.5f; // 点滅の周期（秒単位）
    [SerializeField]
    private TMP_Text text;

    private void Start()
    {
        if (text != null)
        {
            FlashObject().Forget();
        }
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // キーボードの入力を受け取る
        if (!isPressed && keyboard.anyKey.isPressed)
        {
            isPressed = true;
            GameFlowManager.instance.GoToNextScene();
        }
    }

    private async UniTask FlashObject()
    {
        // キャラのスプライトカラーの取得
        Color originalColor = text.color;

        float timer = 0f;

        while (!isPressed)
        {
            // 時間に基づいてアルファ値を滑らかに変化させる
            timer += Time.deltaTime;
            float t = Mathf.PingPong(timer / flashDuration, 1.0f); // 0～1を繰り返す
            float alpha = Mathf.Lerp(minColorAlpha, maxColorAlpha, t);

            // テキストのアルファ値を更新
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            await UniTask.Yield(); // 次のフレームまで待機
        }

        // キャラのスプライトカラーを元に戻す
        text.color = originalColor;
    }
}
