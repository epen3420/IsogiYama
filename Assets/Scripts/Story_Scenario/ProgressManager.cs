using UnityEngine;
using Cysharp.Threading.Tasks;
using IsogiYama.System;
using TMPro;

/*
 * CSVローダーからデータ読み込み
 * Text系コマンド、Button生成系以外はAsyncで非同期処理。Text系はSkip関数が来るまで待機
 * Managerからクリックやスキップなどのユーザー操作が来たら、Commandクラス系にSkip系を呼ぶ.これは非同期処理中でも中断して次進む
 * Command系は読み込んだあと各クラスのExecuteを刺激してコマンドを実行する.
 */

public class ProgressManager : SceneSingleton<ProgressManager>
{
    [SerializeField] private TextAsset file;
    public TextAsset fileProperty 
    {
        get { return file; }
        set { file = value; }
    }

    private CSVLoader csvLoader;
    private CommandFactory commandFactory;

    private CsvData<ScenarioFields> currentScenarioData;
    private int currentIndex;  //今の読み込み列_index
    private int nextIndex; //次読み込む列
    private int totalLine;
    private string currentCommand;

    private void Start()
    {

        currentScenarioData = new CsvData<ScenarioFields>();
        csvLoader = new CSVLoader();
        commandFactory = new CommandFactory();

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
            await commandFactory.CreateCommandInstance(currentCommand).ExecuteAsync(line);

            IncrementIndex();
        }

        Debug.Log($"total:{totalLine} / コマンド終了");
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
        }else Debug.LogWarning("Index Out Of Range");

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
