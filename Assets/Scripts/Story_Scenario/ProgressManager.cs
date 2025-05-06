using UnityEngine;
using Cysharp.Threading.Tasks;
using IsogiYama.System;
using TMPro;

public class ProgressManager : SceneSingleton<ProgressManager>
{
    private TextAsset file;
    public TextAsset fileProperty
    {
        get { return file; }
        set { file = value; }
    }

    private GameFlowManager gameFlowManager;
    private CSVLoader csvLoader;
    private CommandFactory commandFactory;

    private CsvData<ScenarioFields> currentScenarioData;
    private int currentIndex;  //今の読み込み列_index
    private int nextIndex; //次読み込む列
    private int totalLine;
    private string currentCommand;

    private void Start()
    {
        gameFlowManager = GameFlowManager.instance;
        currentScenarioData = new CsvData<ScenarioFields>();
        csvLoader = new CSVLoader();
        commandFactory = new CommandFactory();

        file = gameFlowManager.GetCurrentCSV();
        LoadScenarioData();
        Debug.Log($"total:{totalLine} / コマンド開始");

        ExecuteCommand().Forget(e => Debug.LogError($"ExecuteCommand failed: {e}"));
    }

    public async UniTask ExecuteCommand()
    {
        while (currentIndex < totalLine)
        {
            LineData<ScenarioFields> line = currentScenarioData.Rows[currentIndex];
            currentCommand = line.Get<string>(ScenarioFields.Command);

            CommandBase cmd = commandFactory.CreateCommandInstance(currentCommand);
            if (cmd != null)
            {
                await cmd.ExecuteAsync(line);
            }

            IncrementIndex();
        }

        Debug.Log($"total:{totalLine} / コマンド終了");
        gameFlowManager.GoToNextScene();
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
}
