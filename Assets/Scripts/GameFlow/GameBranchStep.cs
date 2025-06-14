using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 分岐する際に使うGameStep
/// </summary>
[CreateAssetMenu(fileName = "GameBranchStep", menuName = "GameFlow/GameBranchStep")]
public class GameBranchStep : GameStepNeedCSV
{
    [Header("分岐条件と次のステップ")]
    [SerializeField]
    private List<BranchTransitionCondition> transitionConditions;

    private bool isInitialized = false;

    /// <summary>
    /// クリア時間に基づいて次のステップを取得します。
    /// </summary>
    /// <param name="userScore">0から1の実数</param>
    /// <returns></returns>
    public GameStep GetNextGameStepByScore(float userScore)
    {
        InitBranchSteps();

        // 高い minClearScore から順に見ていく（ギリギリ通過できる条件を探す）
        for (int i = transitionConditions.Count - 1; i >= 0; i--)
        {
            var condition = transitionConditions[i];
            if (userScore >= condition.minClearScore)
            {
                return condition.nextStep;
            }
        }

        // どれにも満たない場合は null
        return null;
    }

    /// <summary>
    /// 分岐ステップの初期化処理を行う。
    /// </summary>
    private void InitBranchSteps()
    {
        if (isInitialized) return;

        if (transitionConditions == null || transitionConditions.Count == 0)
        {
            Debug.LogError("Not set next step in branch step");
            return;
        }

        // minClearScore 昇順にソート
        transitionConditions.Sort((x, y) => x.minClearScore.CompareTo(y.minClearScore));
        isInitialized = true;
    }
}
