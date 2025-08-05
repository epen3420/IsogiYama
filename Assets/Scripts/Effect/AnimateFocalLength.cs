using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cysharp.Threading.Tasks;
using System.Threading;

[RequireComponent(typeof(Volume))]
public class AnimateFocalLength : MonoBehaviour
{
    // トランジションにかける時間
    [SerializeField]
    private float transitionDuration = 0.4f;

    // Volumeコンポーネントへの参照
    private Volume globalVolume;

    // DoFの設定を保持する変数
    private DepthOfField depthOfField;

    // 進行中のタスクを管理するためのCancellationTokenSource
    private CancellationTokenSource cancellationTokenSource;

    void Awake()
    {
        globalVolume = GetComponent<Volume>();
    }

    void Start()
    {
        // VolumeコンポーネントからDepthOfField設定を取得
        if (globalVolume.profile.TryGet(out depthOfField))
        {
            // 開始時にFocal Lengthを無効化
            depthOfField.focalLength.overrideState = false;
        }
        else
        {
            Debug.LogError("DepthOfField component not found in the Volume profile. Please ensure it's in the Volume's profile.");
        }

        // CancellationTokenSourceを初期化
        cancellationTokenSource = new CancellationTokenSource();
    }

    void OnDestroy()
    {
        // GameObjectが破棄される際に、進行中のタスクをキャンセル
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
    }

    public async void Transition()
    {
        await TransitionFocalLengthAsync(1f, 300f);
        TransitionFocalLengthAsync(300f, 1f, true).Forget();
    }

    /// <summary>
    /// Focal Lengthを無効化から最大値までトランジションします。
    /// </summary>
    public void TransitionToMax()
    {
        TransitionFocalLengthAsync(1f, 300f).Forget();
    }

    /// <summary>
    /// Focal Lengthを最大値から最低値までトランジションし、その後無効化します。
    /// </summary>
    public void TransitionToMinAndDisable()
    {
        TransitionFocalLengthAsync(300f, 1f, true).Forget();
    }

    /// <summary>
    /// Focal Lengthを徐々に変更する非同期関数
    /// </summary>
    private async UniTask TransitionFocalLengthAsync(float startValue, float endValue, bool disableAfter = false)
    {
        // 既に進行中のタスクがあればキャンセル
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = cancellationTokenSource.Token;

        // DepthOfField設定が見つからない場合は処理を中断
        if (depthOfField == null)
        {
            Debug.LogError("DepthOfField is not initialized.");
            return;
        }

        // Focal Lengthを有効化
        depthOfField.focalLength.overrideState = true;

        // 開始値を設定
        depthOfField.focalLength.value = startValue;

        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            // キャンセルリクエストを検知
            if (token.IsCancellationRequested)
            {
                return;
            }

            float newFocalLength = Mathf.Lerp(startValue, endValue, elapsedTime / transitionDuration);
            depthOfField.focalLength.value = newFocalLength;

            elapsedTime += Time.deltaTime;

            // 次のフレームまで待機
            await UniTask.Yield();
        }

        // 最後の値を正確に設定
        depthOfField.focalLength.value = endValue;

        // トランジション後に無効化する場合
        if (disableAfter)
        {
            depthOfField.focalLength.overrideState = false;
        }
    }
}
