using UnityEngine.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// ポストプロセスのON/OFFを管理するクラス
/// </summary>
public class PostProcessingToggler
{
    private readonly Bloom bloom;
    private readonly FilmGrain filmGrain;
    private readonly ChromaticAberration chromaticAberration;
    private readonly VolumeComponent vintage;

    public PostProcessingToggler(Volume globalVolume)
    {
        if (globalVolume == null)
        {
            Debug.LogWarning("[PostProcessingToggler] Global Volume is null.");
            return;
        }

        var profile = globalVolume.profile;
        profile.TryGet(out bloom);
        profile.TryGet(out filmGrain);
        profile.TryGet(out chromaticAberration);
        profile.TryGet<VolumeComponent>(out vintage);
    }

    public void SetBloom(bool on)
    {
        if (bloom != null)
            bloom.SetAllOverridesTo(on);
    }

    public void SetFilmGrain(bool on)
    {
        if (filmGrain != null)
            filmGrain.SetAllOverridesTo(on);
    }

    public void SetChromaticAberration(bool on)
    {
        if (chromaticAberration != null)
            chromaticAberration.SetAllOverridesTo(on);
    }

    public void SetVintage(bool on)
    {
        if (vintage != null)
            vintage.active = on;
    }

    public void SetAll(bool on)
    {
        SetBloom(on);
        SetFilmGrain(on);
        SetChromaticAberration(on);
        SetVintage(on);
    }
}
