using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenu("Custom/GlitchNoise")]
public class GlitchVolume : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter blockSize = new ClampedFloatParameter(20f, 1f, 100f);
    public ClampedFloatParameter glitchAmount = new ClampedFloatParameter(0.1f, 0f, 1f);
    public ClampedFloatParameter glitchFrequency = new ClampedFloatParameter(1f, 0.1f, 10f);
    public ClampedFloatParameter glitchChance = new ClampedFloatParameter(0.02f, 0f, 1f);
    public ClampedFloatParameter colorSeparation = new ClampedFloatParameter(0.05f, 0f, 0.1f);

    // Volume “à‚Å—LŒø‚©‚Ç‚¤‚©
    public bool IsActive() => glitchAmount.value > 0f && glitchChance.value > 0f;
    public bool IsTileCompatible() => false;
}
