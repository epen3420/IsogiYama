using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背景画像のクロスフェード切り替えを担うクラス
/// </summary>
public class BackgroundFader
{
    private readonly Image mainImage;
    private readonly Image subImage;
    private readonly CanvasGroup vfxCanvasGroup;
    private readonly Dictionary<string, Sprite> spriteLookup;

    public BackgroundFader(Image mainImage, Image subImage, List<Sprite> sprites, CanvasGroup vfxCanvasGroup)
    {
        this.mainImage = mainImage;
        this.subImage = subImage;
        this.vfxCanvasGroup = vfxCanvasGroup;

        // Sprite lookup 初期化
        spriteLookup = new Dictionary<string, Sprite>(sprites.Count);
        foreach (var s in sprites)
        {
            if (s == null) continue;
            spriteLookup[s.name] = s;
        }

        // 初期状態
        this.subImage.gameObject.SetActive(false);
        this.mainImage.color = new Color(1f, 1f, 1f, 1f);
    }

    /// <summary>
    /// 指定キーで背景をクロスフェード切り替え
    /// </summary>
    public async UniTask ChangeBackgroundAsync(string key, float duration = 0.5f)
    {
        if (!spriteLookup.TryGetValue(key, out var target))
        {
            Debug.LogWarning($"[BackgroundFader] Sprite not found: {key}");
            return;
        }

        // Subにセットして有効化
        subImage.sprite = target;
        subImage.gameObject.SetActive(true);
        vfxCanvasGroup.alpha = 1f;

        // フェードアウト→スワップ→フェードイン
        await FadeSwapAsync(duration);
    }

    /// <summary>
    /// メイン画像とサブ画像をフェードアウト・フェードインで切り替え
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    private async UniTask FadeSwapAsync(float duration)
    {
        // Sub の初期透明化
        var subColor = subImage.color;
        subImage.color = new Color(subColor.r, subColor.g, subColor.b, 0f);
        subImage.gameObject.SetActive(true);

        // 同時並行でフェード
        var fadeOutMain = FadeImageAlphaAsync(mainImage, 1f, 0f, duration);
        var fadeInSub = FadeImageAlphaAsync(subImage, 0f, 1f, duration);

        await UniTask.WhenAll(fadeOutMain, fadeInSub);

        // メイン画像を差し替えて Alpha を元に戻す
        mainImage.sprite = subImage.sprite;
        mainImage.color = new Color(mainImage.color.r, mainImage.color.g, mainImage.color.b, 1f);

        subImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// VFX用のCanvasGroupをフェードアウトさせる
    /// </summary>
    /// <param name="canvasGroup"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask FadeOutVFXCanvas(CanvasGroup canvasGroup, float duration)
    {
        float t = 0f;
        // フェードアウト
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - (t / duration);
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// VFX用のCanvasGroupをフェードインさせる
    /// </summary>
    /// <param name="canvasGroup"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask FadeInVFXCanvas(CanvasGroup canvasGroup, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / duration;
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// サブ画像をフェードアウトして非表示にします。
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask HideSubImageFadeAsync(float duration = 0.5f)
    {
        if (!subImage.gameObject.activeSelf) return;

        await FadeImageAlphaAsync(subImage, 1f, 0f, duration);
        subImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// サブ画像をフェードインして表示します。
    /// </summary>
    /// <param name="key"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask ShowSubImageFadeAsync(string key, float duration = 0.5f)
    {
        if (!spriteLookup.TryGetValue(key, out var sprite))
        {
            Debug.LogWarning($"[BackgroundFader] Sprite not found: {key}");
            return;
        }

        subImage.sprite = sprite;
        subImage.gameObject.SetActive(true);
        await FadeImageAlphaAsync(subImage, 0f, 1f, duration);
    }

    /// <summary>
    /// オーバーロード, 指定されたImageのアルファ値をフェードイン・フェードアウトします。
    /// </summary>
    /// <param name="image"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private async UniTask FadeImageAlphaAsync(Image image, float from, float to, float duration)
    {
        if (image == null) return;

        float t = 0f;
        var color = image.color;
        color.a = from;
        image.color = color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / duration);
            image.color = new Color(color.r, color.g, color.b, a);
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        image.color = new Color(color.r, color.g, color.b, to);
    }

    /// <summary>
    /// オーバーロード, 指定されたCanvasGroupのアルファ値をフェードイン・フェードアウトします。
    /// </summary>
    /// <param name="group"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask FadeAlphaAsync(CanvasGroup group, float from, float to, float duration)
    {
        if (group == null) return;

        float t = 0f;
        group.alpha = from;

        while (t < duration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, t / duration);
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        group.alpha = to;
    }

}
