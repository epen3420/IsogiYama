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

    public BranchTransitionCondition GetTransitionConditionByClearTime(float clearTime)
    {
        InitBranchSteps();

        // 最大クリアタイムが一番遅いものより、クリアタイムが遅い場合それを返す
        var bestBadEndCondition = transitionConditions[transitionConditions.Count - 1];
        if (bestBadEndCondition.maxClearTime <= clearTime)
        {
            return bestBadEndCondition;
        }

        foreach (var condition in transitionConditions)
        {
            if (condition.maxClearTime > clearTime)
            {
                return condition;
            }
        }

        return null;
    }

    /// <summary>
    /// クリア時間で昇順にソートとフラグを立てる
    /// </summary>
    private void InitBranchSteps()
    {
        if (isInitialized) return;

        if (transitionConditions == null ||
            transitionConditions.Count == 0)
        {
            Debug.LogError("Not set next step in branch step");
            return;
        }

        // クリア時間を昇順にソート
        transitionConditions.Sort((x, y) => x.maxClearTime.CompareTo(y.maxClearTime));

        isInitialized = true;
    }
}
