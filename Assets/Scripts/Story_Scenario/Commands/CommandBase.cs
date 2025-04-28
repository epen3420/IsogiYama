using Cysharp.Threading.Tasks;
using System.Threading;

public abstract class CommandBase
{
    private CancellationTokenSource cts = new CancellationTokenSource();

    /// <summary>
    /// キャンセルリクエストを送信
    /// </summary>
    public void Cancel()
    {
        if (!cts.IsCancellationRequested)
        {
            cts.Cancel();
        }
    }

    /// <summary>
    /// キャンセルされたか
    /// </summary>
    public bool IsCancelled => cts.IsCancellationRequested;

    /// <summary>
    /// コマンドの実行
    /// </summary>
    public abstract UniTask ExecuteAsync(ScenarioLineData lineData);

    /// <summary>
    /// コマンド実行時に利用するキャンセルトークン
    /// </summary>
    protected CancellationToken Token => cts.Token;
}