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

    // 総ミスタイプ数
    public int TotalIncorrectTypes => mistypedKeys.Values.Sum();

    // エンディング分岐の条件
    public BranchTransitionCondition EndingBranchCondition { get; private set; }

    // パート数を取得するプロパティ
    public int PartCount => partResults.Count;

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

    public void SetEndingBranch(BranchTransitionCondition branch)
    {
        EndingBranchCondition = branch;
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
    public float GetTypingWPS()
    {
        if (ClearTime <= 0) return 0f;
        return TotalCorrectTypes / ClearTime;
    }

    /// <summary>
    /// タイプ速度(文字/分)を計算
    /// </summary>
    public float GetTypingWPM()
    {
        if (ClearTime <= 0) return 0f;
        return (TotalCorrectTypes / ClearTime) * 60f;
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

    /// <summary>
    /// W (1分間あたりのタイプ数) と E (総ミスタイプ数) を基にスコアを計算します。
    /// </summary>
    /// <param name="W">1分間あたりのタイプ数 (文字/分)</param>
    /// <param name="E">総ミスタイプ数</param>
    /// <returns>計算されたスコア（0～1の範囲）</returns>
    private double CalculateScore(double W, double E)
    {
        // Wが非常に小さい値や負の値の場合、ln(W/280)がエラーになるため、適切な範囲チェックを行う
        if (W <= 0)
        {
            Debug.LogWarning("CalculateScore: W (Typing Speed) must be greater than 0.");
            return 0.0;
        }
        if (E < 0)
        {
            Debug.LogWarning("CalculateScore: E (Total Miss Types) cannot be negative.");
            return 0.0;
        }

        // 自然対数の引数が0以下にならないように注意
        double termW = 1.2 * Math.Log(W / 280.0);
        double termE = 3.6 * (E / 60.0);
        double exponent = -(termW - termE + 1.5);

        return 1.0 / (1.0 + Math.Exp(exponent));
    }

    /// <summary>
    /// 現在のTypingResultのデータからスコアを計算します。
    /// </summary>
    /// <returns>計算されたスコア（0～1の範囲）</returns>
    public double GetCurrentScore()
    {
        return CalculateScore(GetTypingWPM(), TotalIncorrectTypes);
    }

    /// <summary>
    /// 指定されたターゲットスコアに到達するために、E (ミスタイプ数) を維持しつつ、
    /// W (タイプ数) をどの程度にする必要があるかを計算します。
    /// </summary>
    /// <param name="targetScore">目標とするスコア (0～1)</param>
    /// <param name="currentE">現在の総ミスタイプ数</param>
    /// <returns>目標スコアを達成するために必要なWの値。達成不可能な場合はNaNを返す。</returns>
    public double GetRequiredWForTargetScore(double targetScore)
    {
        int currentE = TotalIncorrectTypes; 

        if (targetScore <= 0 || targetScore >= 1)
        {
            Debug.LogError("ターゲットは0から1の間で入力してください");
            return double.NaN;
        }

        double inverseScoreTerm = (1.0 / targetScore) - 1.0;
        if (inverseScoreTerm <= 0) // lnの引数が0以下にならないように
        {
            Debug.LogError("Target score too high to reach with this formula's bounds.");
            return double.NaN;
        }

        double X = -Math.Log(inverseScoreTerm); // X = (1.2ln(W/280) -3.6(E/60) + 1.5)

        double E_term = 3.6 * (currentE / 60.0);
        double exponentNumerator = X + E_term - 1.5;
        double requiredW = 280.0 * Math.Exp(exponentNumerator / 1.2);

        return requiredW;
    }

    /// <summary>
    /// 指定されたターゲットスコアに到達するために、W (タイプ数) を維持しつつ、
    /// E (ミスタイプ数) をどの程度にする必要があるかを計算します。
    /// </summary>
    /// <param name="targetScore">目標とするスコア (0～1)</param>
    /// <param name="currentW">現在の1分間あたりのタイプ数</param>
    /// <returns>目標スコアを達成するために必要なEの値。達成不可能な場合はNaNを返す。</returns>
    public double GetRequiredEForTargetScore(double targetScore)
    {
        float currentW = GetTypingWPM();

        if (targetScore <= 0 || targetScore >= 1)
        {
            Debug.LogError("ターゲットは0から1の間で入力してください");
            return double.NaN;
        }
        if (currentW <= 0)
        {
            Debug.LogError("Wが0以下です");
            return double.NaN;
        }

        // スコアの逆関数を解く
        double inverseScoreTerm = (1.0 / targetScore) - 1.0;
        if (inverseScoreTerm <= 0)
        {
            Debug.LogError("Target score too high to reach with this formula's bounds.");
            return double.NaN;
        }

        double X = -Math.Log(inverseScoreTerm); // X = (1.2ln(W/280) -3.6(E/60) + 1.5)

        double W_term = 1.2 * Math.Log(currentW / 280.0);
        double requiredE = 60.0 * (X - W_term - 1.5) / -3.6;

        return requiredE;
    }

    // === デバッグ用表示 ===
    public void PrintSummary()
    {
        Debug.Log($"総タイプ数: {TotalCorrectTypes}");
        Debug.Log($"総クリアタイム: {ClearTime:F2}s");
        Debug.Log($"タイピング速度: {GetTypingWPS():F2} 文字/秒");

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

        // Debug.Log($"エンディング分岐: {EndingBranchCondition.nextStep.CsvFile.name}");
    }
}
