using UnityEngine;
using Cysharp.Threading.Tasks;
using IsogiYama.System;
using System;
using System.Threading;

public class ProgressManager : SceneSingleton<ProgressManager>
{
    [SerializeField]
    private TextAsset file;
    public TextAsset fileProperty
    {
        get { return file; }
        set { file = value; }
    }

    // Monobehaviour
    private GameFlowManager gameFlowManager;
    private VFXController vfxController;

    // utility
    private CSVLoader csvLoader;
    private CommandFactory commandFactory;

    private CsvData<ScenarioFields> currentScenarioData;
    private int currentIndex;  //今の読み込み列_index
    private int nextIndex; //次読み込む列
    private int totalLine;
    private string currentCommand;

    private void Start()
    {
        // 初期化
        currentScenarioData = new CsvData<ScenarioFields>();
        csvLoader = new CSVLoader();
        commandFactory = new CommandFactory();

        try
        {
            vfxController = InstanceRegister.Get<VFXController>();
            gameFlowManager = GameFlowManager.instance;
            file = gameFlowManager.GetCurrentCSV();
        }
        catch (Exception e)
        {
            Debug.LogError($"ProgressManager Start Error: {e}");
        }

        // MonoBehaviour が破棄されたら自動キャンセルされるトークン
        var lifetimeToken = this.GetCancellationTokenOnDestroy();

#if UNITY_WEBGL
    // WebGL 向け
    Debug.Log("WebGL build: Running in SYNC mode");

    LoadScenarioData();
    vfxController.FadeOutCanvasAsync().Forget();
    ExecuteCommand(lifetimeToken)
        .Forget(e => Debug.LogError($"ExecuteCommand failed: {e}"));
#else
    // その他プラットフォーム向け
    Debug.Log("Non-WebGL build: Running in ASYNC mode");

    // 非同期版の初期化を呼び出す
    InitializeAsync(lifetimeToken)
        .Forget(e => Debug.LogError($"InitializeAsync failed: {e}"));
#endif
    }

    public async UniTask ExecuteCommand(CancellationToken ct = default)
    {
        while (currentIndex < totalLine)
        {
            LineData<ScenarioFields> line = currentScenarioData.Rows[currentIndex];
            currentCommand = line.Get<string>(ScenarioFields.Command);

            // Debug.Log($"Read : {currentCommand} / Line : {currentIndex}");

            CommandBase cmd = commandFactory.CreateCommandInstance(currentCommand);
            if (cmd != null)
            {
                // Debug.Log($"Execute : {currentCommand} / Line : {currentIndex}");
                await cmd.ExecuteAsync(line).AttachExternalCancellation(ct); ;
            }

            IncrementIndex();
        }

        Debug.Log("コマンド終了");

        try
        {
            gameFlowManager.GoToNextScene();
        }catch(Exception e)
        {
            Debug.Log($"Error: {e}");
        }
    }

    public void IncrementIndex()
    {
        currentIndex = nextIndex;
        nextIndex = ++nextIndex > totalLine ? totalLine : nextIndex;
    }

    public void IndexSkip(int address = 0)
    {
        if (currentIndex < totalLine && address >= 0)
        {
            nextIndex = address;
        }
        else Debug.LogWarning("Index Out Of Range");

        Debug.Log($"current:{currentIndex} / next:{(nextIndex > totalLine ? totalLine : nextIndex)}");
    }

    public LineData<ScenarioFields> GetIndexLine(int address)
    {
        if (address > totalLine)
        {
            Debug.LogError("Address Out Of Range");
        }
        return currentScenarioData.Rows[address];
    }

    public void LoadScenarioData()
    {
        if (file == null)
        {
            Debug.LogError("File is Null");
            return;
        }

        currentIndex = 0;
        nextIndex = 1;
        currentScenarioData = csvLoader.ReadScenarioCSV(file, "テストファイル");
        totalLine = currentScenarioData.Rows.Count;
    }

    public async UniTask LoadScenarioDataAsync(CancellationToken ct = default)
    {
        if (file == null)
        {
            Debug.LogError("File is Null");
            return;
        }

        currentIndex = 0;
        nextIndex = 1;

        var (isCanceled, data) = await csvLoader
            .LoadCSVAsync<ScenarioFields>(file, ct)
            .SuppressCancellationThrow();

        if (!isCanceled)
        {
            currentScenarioData = data;
            totalLine = currentScenarioData.Rows.Count;
        }
        else
        {
            Debug.LogWarning("シナリオ読み込みがキャンセルされました。");
        }

        totalLine = currentScenarioData.Rows.Count;
        Debug.Log($"[Async] Loaded lines: {totalLine}");
    }

    private async UniTask InitializeAsync(CancellationToken ct = default)
    {
        await LoadScenarioDataAsync(ct);

        // 並列で実行
        var fadeTask = vfxController.FadeOutCanvasAsync().AttachExternalCancellation(ct);;
        var execTask = ExecuteCommand(ct);

        // 両方終わるのを待つ
        await UniTask.WhenAll(fadeTask, execTask);
    }
}
