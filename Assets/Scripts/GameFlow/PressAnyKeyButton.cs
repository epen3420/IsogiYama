using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PressAnyKeyButton : MonoBehaviour
{
    private bool isPressed = false;
    private Keyboard keyboard = null;

    [Header("最小のアルファ値")]
    [SerializeField]
    private float minColorAlpha = 0.5f;
    [Header("最大のアルファ値")]
    [SerializeField]
    private float maxColorAlpha = 1.0f;
    [Header("点滅の周期")]
    [SerializeField]
    private float flashDuration = 0.5f;
    [Header("点滅させるテキスト")]
    [SerializeField]
    private TMP_Text text;


    private async void Start()
    {
        if (text != null)
        {
            FlashObject().Forget();
        }

        keyboard = Keyboard.current;
        // キーが押されてたりした時用に解除されるまで待つ
        await UniTask.WaitUntil(() => !keyboard.anyKey.isPressed);
    }

    private void Update()
    {
        if (isPressed) return;

        keyboard = Keyboard.current;
        if (keyboard == null) return;


        foreach (KeyControl keyControl in keyboard.allKeys)
        {
            if (keyControl == null) return;

            if (keyControl.wasPressedThisFrame &&
                IsPassKey(keyControl.keyCode))
            {
                isPressed = true;
                GameFlowManager.instance.GoToNextScene();
                break;
            }
        }
    }

    private bool IsPassKey(Key key)
    {
        if (key >= Key.A && key <= Key.Z)
            return true;

        if (key >= Key.Digit0 && key <= Key.Digit9)
            return true;

        if (key == Key.Space || key == Key.Enter)
            return true;


        return false;
    }

    /// <summary>
    /// Colorコンポーネントを持つ、オブジェクトを点滅させる
    /// </summary>
    /// <returns></returns>
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
