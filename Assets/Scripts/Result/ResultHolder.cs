using System;
using System.Collections.Generic;
using UnityEngine;

public class ResultHolder : Singleton<ResultHolder>
{
    private TypingResult result = new TypingResult();

    public TypingResult GetResult() => result;

    public void SetResult(TypingResult r) => result = r;

    public void ClearResult() => result = new TypingResult();

    private Dictionary<EndingType, (
    string displayName, // 表示名
    string description, // 説明文
    bool isUnlocked,    // 解放済みか
    bool isSpecial      // 特殊エンディングか
    )> allEndings = new Dictionary<EndingType, (string displayName, string description, bool isUnlocked, bool isSpecial)>();

    public Dictionary<EndingType, (string displayname, string description, bool isUnlocked, bool isSpecial)> GetAllEndings() => allEndings;

    // Endingが更新されたときに呼ばれるイベント
    public static event Action OnEndingsUpdated;

    override public void Awake()
    {
        allEndings.Clear();
        InitializeEndings();
    }

    private void InitializeEndings()
    {
        allEndings.Add(EndingType.ED1, ("ED1", "間に合わなかった...。", false, false));
        allEndings.Add(EndingType.ED2, ("ED2", "気が付いたときには。", false, false));
        allEndings.Add(EndingType.ED3, ("ED3", "無事に山を下りることができた。", false, false));
        allEndings.Add(EndingType.Hidden, ("Hidden", "そういえば...？", false, true));
    }

    /// <summary>
    /// 指定されたエンディングを解放済みに更新します。
    /// 更新があった場合、OnEndingsUpdatedイベントを発行します。
    /// </summary>
    /// <param name="endingName">解放するエンディングの名前</param>
    public void UnlockEnding(EndingType endingName)
    {
        // Dictionaryから指定のエンディングのデータを取り出す
        if (allEndings.TryGetValue(endingName, out var endingData))
        {
            // 既に解放済みでなければ更新する
            if (!endingData.isUnlocked)
            {
                // 解放済みに設定
                endingData.isUnlocked = true;
                // 更新されたタプルをDictionaryに戻す
                allEndings[endingName] = endingData;

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
            Debug.LogWarning($"Ending '{endingName}' not found in the list.");
        }
    }

    /// <summary>
    /// 特定のエンディングが解放済みかどうかをチェックします。
    /// </summary>
    /// <param name="endingName">チェックするエンディングの名前</param>
    /// <returns>エンディングが解放済みであればtrue、そうでなければfalse。</returns>
    public bool IsEndingUnlocked(EndingType endingName)
    {
        if (allEndings.TryGetValue(endingName, out var endingData))
        {
            return endingData.isUnlocked;
        }
        // エンディングが見つからない場合は未解放とみなす
        return false;
    }
}
