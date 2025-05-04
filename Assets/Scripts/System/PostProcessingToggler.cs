using UnityEngine.Rendering.Universal;

using UnityEngine.Rendering;

/// <summary>
/// ポストプロセスのON/OFFとパラメータ調整を行うクラス
/// </summary>
public class PostProcessingToggler
{
    private readonly Bloom bloom;
    private readonly FilmGrain filmGrain;
    private readonly ChromaticAberration chromaticAberration;
    private readonly DepthOfField depthOfField;
    private readonly LensDistortion lensDistortion;
    private readonly VolumeComponent vintage;

    public PostProcessingToggler(Volume globalVolume)
    {
        var profile = globalVolume.profile;
        profile.TryGet(out bloom);
        profile.TryGet(out filmGrain);
        profile.TryGet(out chromaticAberration);
        profile.TryGet(out depthOfField);
        profile.TryGet(out lensDistortion);
        profile.TryGet<VolumeComponent>(out vintage);
    }

    // Bloom
    public void SetBloomEnabled(bool on)
    {
        if (bloom != null) bloom.active = on;
    }
    public void SetBloomParameters(float intensity, float threshold, float scatter)
    {
        if (bloom == null) return;
        bloom.intensity.value = intensity;
        bloom.threshold.value = threshold;
        bloom.scatter.value = scatter;
    }

    // Film Grain
    public void SetFilmGrainEnabled(bool on)
    {
        if (filmGrain != null) filmGrain.active = on;
    }
    public void SetFilmGrainParameters(float intensity, float response)
    {
        if (filmGrain == null) return;
        filmGrain.intensity.value = intensity;
        filmGrain.response.value = response;
    }

    // Chromatic Aberration
    public void SetChromaticAberrationEnabled(bool on)
    {
        if (chromaticAberration != null) chromaticAberration.active = on;
    }
    public void SetChromaticAberrationParameters(float intensity)
    {
        if (chromaticAberration == null) return;
        chromaticAberration.intensity.value = intensity;
    }

    // Depth of Field
    public void SetDepthOfFieldEnabled(bool on)
    {
        if (depthOfField != null) depthOfField.active = on;
    }
    public void SetDepthOfFieldParameters(float focusDistance, float aperture, float focalLength)
    {
        if (depthOfField == null) return;
        depthOfField.focusDistance.value = focusDistance;
        depthOfField.aperture.value = aperture;
        depthOfField.focalLength.value = focalLength;
    }

    // Lens Distortion
    public void SetLensDistortionEnabled(bool on)
    {
        if (lensDistortion != null) lensDistortion.active = on;
    }
    public void SetLensDistortionParameters(float intensity, float scale)
    {
        if (lensDistortion == null) return;
        lensDistortion.intensity.value = intensity;
        lensDistortion.scale.value = scale;
    }

    // Vintage (カスタム)
    public void SetVintageEnabled(bool on)
    {
        if (vintage != null) vintage.active = on;
    }
    public void SetVintageParameter<T>(string fieldName, float value) where T : VolumeComponent
    {
        if (vintage == null) return;
        var component = vintage as VolumeComponent;
        var field = component.GetType().GetField(fieldName);
        if (field != null)
        {
            var param = field.GetValue(component) as VolumeParameter<float>;
            if (param != null) param.value = value;
        }
    }

    /// <summary>
    /// 全エフェクトのON/OFF
    /// </summary>
    public void SetAllEnabled(bool on)
    {
        SetBloomEnabled(on);
        SetFilmGrainEnabled(on);
        SetChromaticAberrationEnabled(on);
        SetDepthOfFieldEnabled(on);
        SetLensDistortionEnabled(on);
        SetVintageEnabled(on);
    }
}