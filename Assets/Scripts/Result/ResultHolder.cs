using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResultHolder : Singleton<ResultHolder>
{
    private TypingResult result = new TypingResult();

    public TypingResult GetResult() => result;

    public void SetResult(TypingResult r) => result = r;

    public void ClearResult() => result = new TypingResult();

    [SerializeField]
    private List<EndingEntry> endingsList = new();

    // エンディングのリストをDictionaryに変換して保持
    private Dictionary<EndingType, EndingEntry> endingsDict;

    public Dictionary<EndingType, EndingEntry> GetAllEndings() => endingsDict;

    // Endingが更新されたときに呼ばれるイベント
    public static event Action OnEndingsUpdated;

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


    override public void Awake()
    {
        endingsDict = endingsList.ToDictionary(e => e.Key);
    }
}
