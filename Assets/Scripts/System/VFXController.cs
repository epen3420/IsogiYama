using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;

public class VFXController : SceneSingleton<VFXController>
{
    [Header("Background Components (Inspector)")]
    [SerializeField] private Image backgroundImageMain;
    [SerializeField] private Image backgroundImageSub;
    [SerializeField] private CanvasGroup vfxCanvasCanvasGroup;
    [SerializeField] private CanvasGroup overlayCanvasGroup;
    [SerializeField] private List<Sprite> backgroundSprites;

    [Header("Post Processing (Inspector)")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private Material glitchMaterial;

    private BackgroundFader bgFader;
    private PostProcessingToggler postToggler;
    // CameraController のインスタンス
    private CameraController cameraController;
    private bool isTransitioning = false;
    private bool isFading = false;

    protected override void Awake()
    {
        base.Awake();
        var mainCam = Camera.main;

        bgFader = new BackgroundFader(
            backgroundImageMain,
            backgroundImageSub,
            backgroundSprites,
            vfxCanvasCanvasGroup
        );

        postToggler = new PostProcessingToggler(globalVolume);

        cameraController = new CameraController(mainCam);
    }

    /// <summary>
    /// 背景画像をクロスフェードで切り替えます。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="duration">フェード時間（秒）</param>
    /// <returns></returns>
    public async UniTask ChangeBackgroundAsync(string key, float duration = 0.5f)
    {
        if (isTransitioning) return;
        isTransitioning = true;
        try { await bgFader.ChangeBackgroundAsync(key, duration); }
        finally { isTransitioning = false; }
    }

    /// <summary>
    /// VFXCanvasをフェードアウトします。
    /// </summary>
    /// <param name="duration">フェード時間（秒）</param>
    /// <returns></returns>
    public async UniTask FadeOutCanvasAsync(float duration = 0.5f)
    {
        if (isFading) return;
        isFading = true;
        try { await bgFader.FadeOutVFXCanvas(overlayCanvasGroup, duration); }
        finally { isFading = false; }
    }

    /// <summary>
    /// VFXCanvasをフェードインします。
    /// </summary>
    /// <param name="duration">フェード時間（秒）</param>
    /// <returns></returns>
    public async UniTask FadeInCanvasAsync(float duration = 0.5f)
    {
        if (isFading) return;
        isFading = true;
        try { await bgFader.FadeInVFXCanvas(overlayCanvasGroup, duration); }
        finally { isFading = false; }
    }

    /// <summary>
    /// Sub側の背景画像をフェード表示します。
    /// </summary>
    /// <param name="key">表示したいスプライト名</param>
    /// <param name="duration">フェード時間（秒）</param>
    public async UniTask ShowSubBackgroundAsync(string key, float duration = 0.5f, float endValue = 1f)
    {
        await bgFader.ShowSubImageFadeAsync(key, duration, endValue);
    }

    /// <summary>
    /// Sub側の背景画像をフェード非表示にします。
    /// </summary>
    /// <param name="duration">フェード時間（秒）</param>
    public async UniTask HideSubBackgroundAsync(float duration = 0.5f)
    {
        await bgFader.HideSubImageFadeAsync(duration);
    }


    public void SetBloom(bool on) => postToggler.SetBloomEnabled(on);
    public void SetBloomParameters(float intensity, float threshold, float scatter) => postToggler.SetBloomParameters(intensity, threshold, scatter);

    public void SetFilmGrain(bool on) => postToggler.SetFilmGrainEnabled(on);
    public void SetFilmGrainParameters(float intensity, float response) => postToggler.SetFilmGrainParameters(intensity, response);

    public void SetChromaticAberration(bool on) => postToggler.SetChromaticAberrationEnabled(on);
    public void SetChromaticAberrationParameters(float intensity) => postToggler.SetChromaticAberrationParameters(intensity);

    public void SetDepthOfField(bool on) => postToggler.SetDepthOfFieldEnabled(on);
    public void SetDepthOfFieldParameters(float focusDistance, float aperture, float focalLength) => postToggler.SetDepthOfFieldParameters(focusDistance, aperture, focalLength);

    public void SetLensDistortion(bool on) => postToggler.SetLensDistortionEnabled(on);
    public void SetLensDistortionParameters(float intensity, float scale) => postToggler.SetLensDistortionParameters(intensity, scale);

    public void SetVintage(bool on) => postToggler.SetVintageEnabled(on);
    public void SetVintageParameter<T>(string fieldName, float value) where T : VolumeComponent => postToggler.SetVintageParameter<T>(fieldName, value);

    public void SetAllPostProcessing(bool on) => postToggler.SetAllEnabled(on);

    /// <summary>
    /// カメラを振動させる
    /// </summary>
    /// <param name="duration">振動時間(秒)</param>
    /// <param name="magnitude">振動強度</param>
    /// <returns></returns>
    public async UniTask ShakeCameraAsync(float duration, float magnitude)
    {
        if(cameraController == null)
        {
            Debug.LogWarning("CameraController is not initialized");
            return;
        }

        await cameraController.ShakeAsync(duration, magnitude);
    }

    /// <summary>
    /// 背景イメージにグリッチマテリアルを適用or解除します
    /// </summary>
    /// <param name="on">true: グリッチON / false: グリッチOFF</param>
    public void SetGlitch(bool on)
    {
        if (backgroundImageMain == null)
            return;

        if (on && glitchMaterial != null)
        {
            backgroundImageMain.material = glitchMaterial;
        }
        else
        {
            // None にしたい場合は null、もしくは元のマテリアルに戻す
            backgroundImageMain.material = null;
        }
    }

    /// <summary>
    /// 背景画像を振動させる
    /// </summary>
    /// <param name="duration">振動時間(秒)</param>
    /// <param name="magnitude">振動強度</param>
    public async UniTask ShakeBackgroundAsync(float duration, float magnitude)
    {
        if (backgroundImageMain == null)
        {
            Debug.LogWarning("backgroundImageMain が設定されていません");
            return;
        }

        // RectTransform を取得
        var rt = backgroundImageMain.rectTransform;
        // 元の位置をキャッシュ
        var originalPos = rt.anchoredPosition;

        float elapsed = 0f;
        // 振動ループ
        while (elapsed < duration)
        {
            // ランダムなオフセット
            var offset = Random.insideUnitCircle * magnitude;
            rt.anchoredPosition = originalPos + offset;

            // 次フレームまで待機
            await UniTask.Yield(PlayerLoopTiming.Update);

            elapsed += Time.deltaTime;
        }

        // 終了後、元の位置に戻す
        rt.anchoredPosition = originalPos;
    }

    /// <summary>
    /// 指定されたTMP_Textのテキストのアルファ値を設定します。
    /// </summary>
    /// <param name="tmp_Text"></param>
    /// <param name="alpha"></param>
    public void SetTextAlpha(TMP_Text tmp_Text, float alpha)
    {
        if (tmp_Text == null) return;
        var color = tmp_Text.color;
        color.a = alpha;
        tmp_Text.color = color;
    }

    /// <summary>
    /// 指定されたTMP_Textのテキストをフェードインさせます。
    /// </summary>
    /// <param name="textComp">フェードアウトさせるTMP_Textコンポーネント</param>
    /// <param name="duration">フェードアウトにかける時間（秒）</param>
    /// <param name="token">キャンセルトークン。操作を中断する際に使用します</param>
    /// <returns></returns>
    public async UniTask FadeInText(TMP_Text textComp, float duration, CancellationToken token)
    {
        textComp.gameObject.SetActive(true); // フェードイン前にEnabledをtrueにする
        var col = textComp.color;
        col.a = 0f;
        textComp.color = col;
        for (float t = 0; t <= duration; t += Time.deltaTime)
        {
            token.ThrowIfCancellationRequested();
            col.a = Mathf.Lerp(0f, 1f, t / duration);
            textComp.color = col;
            await UniTask.Yield(token);
        }
        col.a = 1f;
        textComp.color = col;
    }

    /// <summary>
    /// 指定されたTMP_Textのテキストをフェードアウトさせます。
    /// </summary>
    /// <param name="textComp">フェードアウトさせるTMP_Textコンポーネント</param>
    /// <param name="duration">フェードアウトにかける時間（秒）</param>
    /// <param name="token">キャンセルトークン。操作を中断する際に使用します</param>
    /// <param name="beEnabled">フェードアウトしたあとEnabledをbeEnabledにします。</param>
    /// <returns></returns>
    public async UniTask FadeOutText(TMP_Text textComp, float duration, bool beEnabled, CancellationToken token)
    {
        textComp.enabled = true; // フェードアウト前にEnabledをtrueにする
        var col = textComp.color;
        col.a = 1f;
        textComp.color = col;
        for (float t = 0; t <= duration; t += Time.deltaTime)
        {
            token.ThrowIfCancellationRequested();
            col.a = Mathf.Lerp(1f, 0f, t / duration);
            textComp.color = col;
            await UniTask.Yield(token);
        }
        col.a = 0f;
        textComp.color = col;
        textComp.gameObject.SetActive(beEnabled); // フェードアウト後にEnabledを設定
    }
}
