using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class VFXController : SceneSingleton<VFXController>
{
    [Header("Background Components (Inspector)")]
    [SerializeField] private Image backgroundImageMain;
    [SerializeField] private Image backgroundImageSub;
    [SerializeField] private CanvasGroup backgroundCanvasGroup;
    [SerializeField] private List<Sprite> backgroundSprites;

    [Header("Post Processing (Inspector)")]
    [SerializeField] private Volume globalVolume;

    private BackgroundFader bgFader;
    private PostProcessingToggler postToggler;
    private bool isTransitioning = false;

    protected override void Awake()
    {
        base.Awake();

        bgFader = new BackgroundFader(
            backgroundImageMain,
            backgroundImageSub,
            backgroundCanvasGroup,
            backgroundSprites
        );

        postToggler = new PostProcessingToggler(globalVolume);
    }

    public async UniTask ChangeBackgroundAsync(string key, float duration = 0.5f)
    {
        if (isTransitioning) return;
        isTransitioning = true;
        try { await bgFader.ChangeBackgroundAsync(key, duration); }
        finally { isTransitioning = false; }
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
}