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
    private readonly CanvasGroup canvasGroup;
    private readonly Dictionary<string, Sprite> spriteLookup;

    public BackgroundFader(Image mainImage, Image subImage, CanvasGroup canvasGroup, List<Sprite> sprites)
    {
        this.mainImage = mainImage;
        this.subImage = subImage;
        this.canvasGroup = canvasGroup;

        // Sprite lookup 初期化
        spriteLookup = new Dictionary<string, Sprite>(sprites.Count);
        foreach (var s in sprites)
        {
            if (s == null) continue;
            spriteLookup[s.name] = s;
        }

        // 初期状態
        this.subImage.gameObject.SetActive(false);
        this.canvasGroup.alpha = 1f;
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

        // フェードアウト→スワップ→フェードイン
        await FadeSwapAsync(duration);
    }

    private async UniTask FadeSwapAsync(float duration)
    {
        float t = 0f;
        // フェードアウト(Main)
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - (t / duration);
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        canvasGroup.alpha = 0f;

        // MainにSubのSpriteを同期
        mainImage.sprite = subImage.sprite;

        // フェードイン(Main)
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / duration;
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        canvasGroup.alpha = 1f;

        // Subを非表示に戻す
        subImage.gameObject.SetActive(false);
    }
}