using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

/// <summary>
/// CanvasGroup のアルファを周期的に点滅させ、
/// 停止時には滑らかにフェードアウトして非アクティブ化するクラス。
/// </summary>
public class IdleLogoBlink : IDisposable
{
    private readonly CanvasGroup _canvasGroup;
    private readonly float _blinkPeriod;
    private readonly float _minAlpha;
    private readonly float _fadeDuration = 0.5f; // フェードイン／アウト時間（秒）

    private CancellationTokenSource _cts;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="canvasGroup">点滅・非表示対象の CanvasGroup</param>
    /// <param name="blinkPeriod">点滅の周期（秒）</param>
    /// <param name="minAlpha">点滅時の最小アルファ値（0〜1）</param>
    public IdleLogoBlink(CanvasGroup canvasGroup, float blinkPeriod, float minAlpha)
    {
        _canvasGroup = canvasGroup ?? throw new ArgumentNullException(nameof(canvasGroup));
        _blinkPeriod = blinkPeriod;
        _minAlpha = Mathf.Clamp01(minAlpha);
    }

    /// <summary>
    /// 点滅を開始します。
    /// フェードイン後に点滅ループを開始。
    /// </summary>
    public void StartBlink()
    {
        // 既存の停止処理が走っている場合はキャンセル
        _cts?.Cancel();

        // CanvasGroup を有効化し、透明からスタート
        _canvasGroup.gameObject.SetActive(true);
        _canvasGroup.alpha = 0f;

        _cts = new CancellationTokenSource();
        FadeInAndStartBlink(_cts.Token).Forget();
    }

    /// <summary>
    /// 点滅を停止し、滑らかにフェードアウトした後に非アクティブ化します。
    /// </summary>
    public void StopBlink()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        // フェードアウト処理
        FadeOutAndDeactivate().Forget();
    }

    private async UniTaskVoid FadeInAndStartBlink(CancellationToken ct)
    {
        float elapsed = 0f;
        while (elapsed < _fadeDuration && !ct.IsCancellationRequested)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
            elapsed += Time.deltaTime;
            var t = Mathf.Clamp01(elapsed / _fadeDuration);
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
        }

        // 完全に不透明に
        _canvasGroup.alpha = 1f;

        // フェードイン後に点滅ループ開始
        await UniTask.Yield(PlayerLoopTiming.Update);
        BlinkLoop(ct).Forget();
    }

    private async UniTaskVoid BlinkLoop(CancellationToken ct)
    {
        float t = 0f;
        while (!ct.IsCancellationRequested)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
            t += Time.deltaTime * (2 * Mathf.PI / _blinkPeriod);
            if (t > 2 * Mathf.PI) t -= 2 * Mathf.PI;

            // sin 波 [0→1], minAlpha～1 を線形補間
            var sin = (Mathf.Sin(t) + 1f) / 2f;
            _canvasGroup.alpha = Mathf.Lerp(_minAlpha, 1f, sin);
        }
    }

    private async UniTaskVoid FadeOutAndDeactivate()
    {
        float startAlpha = _canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
            elapsed += Time.deltaTime;
            var t = Mathf.Clamp01(elapsed / _fadeDuration);
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
        }

        _canvasGroup.alpha = 0f;
        _canvasGroup.gameObject.SetActive(false);
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
