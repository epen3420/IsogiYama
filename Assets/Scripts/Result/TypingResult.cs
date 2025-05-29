using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TypingResult
{
    // パートごとの正解タイプ数とミスタイプ数(indexは0-based)
    private List<(int correctCount, int missCount, float partTime)> partResults = new();

    // ミスタイプしたキーとその回数
    private Dictionary<char, int> mistypedKeys = new();

    // 総正解タイプ数
    public int TotalCorrectTypes { get; private set; }

    // エンディング分岐
    public int EndingBranch { get; private set; } = -1;

    /// <summary>
    /// 総クリアタイム（パートの合計時間）
    /// </summary>
    public float ClearTime => partResults.Sum(result => result.partTime);

    // === 登録・設定 ===

    public void AddPartResult(int correctCount, int missCount, float partTime)
    {
        partResults.Add((correctCount, missCount, partTime));
        TotalCorrectTypes += correctCount;
    }

    public void AddMistypedKey(char key)
    {
        if (mistypedKeys.ContainsKey(key))
            mistypedKeys[key]++;
        else
            mistypedKeys[key] = 1;
    }

    public void SetEndingBranch(int branch)
    {
        EndingBranch = branch;
    }

    // === 取得 ===

    /// <summary>
    /// ワーストミスキーTop3(ミス数降)
    /// </summary>
    public char[] GetWorstMistypedKeys(int topN = 3)
    {
        return mistypedKeys
            .OrderByDescending(pair => pair.Value)
            .Take(topN)
            .Select(pair => pair.Key)
            .ToArray();
    }

    /// <summary>
    /// タイプ速度(文字/秒)を計算
    /// </summary>
    public float GetTypingSpeed()
    {
        if (ClearTime <= 0) return 0f;
        return TotalCorrectTypes / ClearTime;
    }

    /// <summary>
    /// 指定したパートの(クリアタイム, ミスタイプ数)を返す
    /// </summary>
    public (float partTime, int missCount) GetPartInfo(int partIndex)
    {
        if (partIndex < 0 || partIndex >= partResults.Count)
            throw new ArgumentOutOfRangeException(nameof(partIndex), "無効なパート番号です");

        var result = partResults[partIndex];
        return (result.partTime, result.missCount);
    }

    // === デバッグ用表示 ===
    public void PrintSummary()
    {
        Debug.Log($"総タイプ数: {TotalCorrectTypes}");
        Debug.Log($"総クリアタイム: {ClearTime:F2}s");
        Debug.Log($"タイピング速度: {GetTypingSpeed():F2} 文字/秒");

        for (int i = 0; i < partResults.Count; i++)
        {
            var (correct, miss, time) = partResults[i];
            Debug.Log($"Part {i + 1}: 正解 {correct}, ミス {miss}, 時間 {time:F2}s");
        }

        var worst = GetWorstMistypedKeys();
        if (worst.Length == 0)
        {
            Debug.Log("ワーストキー: なし");
        }
        else
        {
            Debug.Log($"ワーストキー: {string.Join(", ", worst.Select(k => k.ToString()))}");
        }

        Debug.Log($"エンディング分岐: ed{EndingBranch}_perf.csv");
    }
}
