using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// VFX全体の窓口: 背景切り替えとポストプロセス制御を統括
/// </summary>
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

    protected override void Awake()
    {
        base.Awake();

        // DIして純粋クラスを初期化
        bgFader = new BackgroundFader(
            backgroundImageMain,
            backgroundImageSub,
            backgroundCanvasGroup,
            backgroundSprites
        );

        postToggler = new PostProcessingToggler(globalVolume);
    }

    /// <summary>
    /// 背景切り替えを外部から呼び出し
    /// </summary>
    public async UniTask ChangeBackgroundAsync(string key, float duration = 0.5f)
    {
        await bgFader.ChangeBackgroundAsync(key, duration);
    }

    /// <summary>
    /// Bloom ON/OFF
    /// </summary>
    public void SetBloom(bool on) => postToggler.SetBloom(on);
    public void SetFilmGrain(bool on) => postToggler.SetFilmGrain(on);
    public void SetChromaticAberration(bool on) => postToggler.SetChromaticAberration(on);
    public void SetVintage(bool on) => postToggler.SetVintage(on);

    /// <summary>
    /// 全エフェクトまとめてON/OFF
    /// </summary>
    public void SetAllPostProcessing(bool on) => postToggler.SetAll(on);
}
