using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VFXController : SceneSingleton<VFXController>
{
    [Header("UI Components (Inspectorでセット)")]
    [Tooltip("背景表示用のImage")]
    [SerializeField] private Image backgroundImage;
    [Tooltip("Image を包む CanvasGroup")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Background Sprites")]
    [Tooltip("エディタ上で分かりやすい名前にリネームした Sprite を登録")]
    [SerializeField] private List<Sprite> backgroundSprites = new List<Sprite>();

    [Header("Post Processing")]
    [Tooltip("Global Volume (Volume Profile に各種エフェクトを登録しておく)")]
    [SerializeField] private Volume globalVolume;

    // 内部参照
    private Dictionary<string, Sprite> spriteLookup;
    private Bloom bloom;
    private FilmGrain filmGrain;
    private ChromaticAberration chromaticAberration;
    private VolumeComponent vintage; // カスタムエフェクト

    protected override void Awake()
    {
        base.Awake();

        // CanvasGroup と Image は Inspector からセット済み
        if (backgroundImage == null || canvasGroup == null)
            Debug.LogError("[VFXController] CanvasGroup または Image がセットされていません。");

        InitializeLookup();
        CachePostProcessOverrides();
    }

    /// <summary>
    /// Sprite リストを名前→Sprite 辞書に変換
    /// </summary>
    private void InitializeLookup()
    {
        spriteLookup = new Dictionary<string, Sprite>(backgroundSprites.Count);
        foreach (var sprite in backgroundSprites)
        {
            if (sprite == null) continue;
            spriteLookup[sprite.name] = sprite;
        }
    }

    /// <summary>
    /// VolumeProfile から各エフェクトの参照を取得
    /// </summary>
    private void CachePostProcessOverrides()
    {
        if (globalVolume == null)
        {
            Debug.LogWarning("[VFXController] Global Volume がセットされていません。");
            return;
        }

        var profile = globalVolume.profile;
        profile.TryGet(out bloom);
        profile.TryGet(out filmGrain);
        profile.TryGet(out chromaticAberration);
        // Vintage はカスタム VolumeComponent として追加してある前提
        profile.TryGet<VolumeComponent>(out vintage);
    }

    /// <summary>
    /// 背景を切り替える（フェード有り）
    /// </summary>
    public void ChangeBackground(string fileName)
    {
        if (!spriteLookup.TryGetValue(fileName, out var target))
        {
            Debug.LogWarning($"[VFXController] Sprite not found: {fileName}");
            return;
        }

        if (canvasGroup != null)
        {
            FadeSwapAsync(target).Forget();
        }
        else
        {
            backgroundImage.sprite = target;
        }
    }

    private async UniTaskVoid FadeSwapAsync(Sprite newSprite)
    {
        const float duration = 0.5f;
        float t = 0f;
        // フェードアウト
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - (t / duration);
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        canvasGroup.alpha = 0f;

        backgroundImage.sprite = newSprite;

        // フェードイン
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / duration;
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        canvasGroup.alpha = 1f;
    }

    // --- Post Processing Control ---

    public void SetBloom(bool on)
    {
        if (bloom != null) bloom.active = on;
    }

    public void SetFilmGrain(bool on)
    {
        if (filmGrain != null) filmGrain.active = on;
    }

    public void SetChromaticAberration(bool on)
    {
        if (chromaticAberration != null) chromaticAberration.active = on;
    }

    public void SetVintage(bool on)
    {
        if (vintage != null) vintage.active = on;
    }

    /// <summary>
    /// 全エフェクトをまとめて有効／無効
    /// </summary>
    public void SetAllPostProcessing(bool on)
    {
        SetBloom(on);
        SetFilmGrain(on);
        SetChromaticAberration(on);
        SetVintage(on);
    }
}
