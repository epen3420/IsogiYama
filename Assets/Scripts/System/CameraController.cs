using Cysharp.Threading.Tasks;
using UnityEngine;

public class CameraController
{
    private readonly Camera camera;
    private readonly Transform cameraTransform;
    private readonly Vector3 initialPosition;

    /// <summary>
    /// コンストラクタで Camera を受け取り、初期位置を記憶します。
    /// </summary>
    public CameraController(Camera cam)
    {
        camera = cam ?? throw new System.ArgumentNullException(nameof(cam));
        cameraTransform = cam.transform;
        initialPosition = cameraTransform.localPosition;
    }

    /// <summary>
    /// 指定時間 duration 秒、強度 magnitude でカメラを振動させます。
    /// </summary>
    public async UniTask ShakeAsync(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // ランダムなオフセットを生成
            Vector3 offset = Random.insideUnitSphere * magnitude;
            cameraTransform.localPosition = initialPosition + offset;

            // 次フレームまで待機
            await UniTask.Yield(PlayerLoopTiming.Update);
            elapsed += Time.deltaTime;
        }

        // 振動終了後は必ず初期位置に戻す
        cameraTransform.localPosition = initialPosition;
    }
}
