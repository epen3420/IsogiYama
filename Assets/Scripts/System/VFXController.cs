using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class VFXController : SceneSingleton<VFXController>
{
    [Header("Background Components (Inspector)")]
    [SerializeField] private Image backgroundImageMain;
    [SerializeField] private Image backgroundImageSub;
    [SerializeField] private CanvasGroup backgroundCanvasGroup;
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
            backgroundCanvasGroup,
            backgroundSprites,
            vfxCanvasCanvasGroup
        );

        postToggler = new PostProcessingToggler(globalVolume);

        cameraController = new CameraController(mainCam);
    }

    public async UniTask ChangeBackgroundAsync(string key, float duration = 0.5f)
    {
        if (isTransitioning) return;
        isTransitioning = true;
        try { await bgFader.ChangeBackgroundAsync(key, duration); }
        finally { isTransitioning = false; }
    }

    public async UniTask FadeOutCanvasAsync(float duration = 0.5f)
    {
        if (isFading) return;
        isFading = true;
        try { await bgFader.FadeOutVFXCanvas(overlayCanvasGroup, duration); }
        finally { isFading = false; }
    }

    public async UniTask FadeInCanvasAsync(float duration = 0.5f)
    {
        if (isFading) return;
        isFading = true;
        try { await bgFader.FadeInVFXCanvas(overlayCanvasGroup, duration); }
        finally { isFading = false; }
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
}