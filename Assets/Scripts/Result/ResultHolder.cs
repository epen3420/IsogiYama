using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ResultHolder : Singleton<ResultHolder>
{
    private TypingResult result = new TypingResult();

    public TypingResult GetResult() => result;

    public void SetResult(TypingResult r) => result = r;

    [SerializeField]
    private List<EndingEntry> endingsList = new();

    // エンディングのリストをDictionaryに変換して保持
    private Dictionary<EndingType, EndingEntry> endingsDict;

    public Dictionary<EndingType, EndingEntry> GetAllEndings() => endingsDict;

    // Endingが更新されたときに呼ばれるイベント
    public static event Action OnEndingsUpdated;

    // CSVファイルのパス
    private string _csvFilePath;

    /// <summary>
    /// 現在のTypingResultデータをCSVに保存し、新しい結果オブジェクトを作成します。
    /// </summary>
    public void ClearResult()
    {
        // 現在の結果が空でない場合のみ保存
        if (result.PartCount > 0)
        {
            SaveResultToCsv(result);
        }

        // 新しい結果オブジェクトを作成してリセット
        result = new TypingResult();
    }

    /// <summary>
    /// 指定されたエンディングを解放済みに更新します。
    /// 更新があった場合、OnEndingsUpdatedイベントを発行します。
    /// </summary>
    /// <param name="endingName">解放するエンディングの名前</param>
    public void UnlockEnding(EndingType endingName)
    {
        if (endingsDict.TryGetValue(endingName, out var endingData))
        {
            if (!endingData.isUnlocked)
            {
                endingData.isUnlocked = true;

                Debug.Log($"Ending '{endingName}' unlocked.");
                OnEndingsUpdated?.Invoke();
            }
            else
            {
                Debug.Log($"Ending '{endingName}' is already unlocked.");
            }
        }
        else
        {
            Debug.LogWarning($"Ending '{endingName}' not found in the dictionary.");
        }
    }

    /// <summary>
    /// 特定のエンディングが解放済みかどうかをチェックします。
    /// </summary>
    /// <param name="endingName">チェックするエンディングの名前</param>
    /// <returns>エンディングが解放済みであればtrue、そうでなければfalse。</returns>
    public bool IsEndingUnlocked(EndingType endingName)
    {
        if (endingsDict.TryGetValue(endingName, out var endingData))
        {
            return endingData.isUnlocked;
        }
        return false;
    }

    public override void Awake()
    {
        base.Awake();
        endingsDict = endingsList.ToDictionary(e => e.Key);
    }

    private void Start()
    {
        _csvFilePath = Path.Combine(Application.persistentDataPath, "typing_results.csv");
        CheckAndCreateCsvFile();
    }

    /// <summary>
    /// CSVファイルが存在しない場合はヘッダー付きで作成します。
    /// </summary>
    private void CheckAndCreateCsvFile()
    {
        if (!File.Exists(_csvFilePath))
        {
            Debug.Log($"CSVファイルが存在しないため、新規作成します: {_csvFilePath}");
            var header = "Index,Version,Timestamp,TotalCorrectTypes,TotalIncorrectTypes,ClearTime,TypingWPM,EndingType,CalculatedScore";
            File.WriteAllText(_csvFilePath, header + Environment.NewLine);
        }
    }

    /// <summary>
    /// TypingResultの情報をCSVに追記します。
    /// </summary>
    /// <param name="resultToSave">保存するTypingResultオブジェクト</param>
    private void SaveResultToCsv(TypingResult resultToSave)
    {
        try
        {
            // インデックスの決定
            int newIndex = 1;
            if (File.Exists(_csvFilePath))
            {
                var lines = File.ReadAllLines(_csvFilePath);
                if (lines.Length > 1)
                {
                    // 最後の行からインデックスを取得して+1する
                    var lastLine = lines.Last();
                    var lastIndexStr = lastLine.Split(',').FirstOrDefault();
                    if (int.TryParse(lastIndexStr, out int lastIndex))
                    {
                        newIndex = lastIndex + 1;
                    }
                }
            }

            // CSV行のデータを作成
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string line = $"{newIndex},{Application.version},{timestamp}," +
                          $"{resultToSave.TotalCorrectTypes}," +
                          $"{resultToSave.TotalIncorrectTypes}," +
                          $"{resultToSave.ClearTime:F2}," +
                          $"{resultToSave.GetTypingWPM():F2}," +
                          $"{resultToSave.EndingType}," +
                          $"{resultToSave.GetCurrentScore():F4}";

            // ファイルに追記
            File.AppendAllText(_csvFilePath, line + Environment.NewLine);
            Debug.Log($"CSVに新しい結果を保存しました。ファイルパス: {_csvFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"CSVファイルへの書き込み中にエラーが発生しました: {e.Message}");
        }
    }
}
