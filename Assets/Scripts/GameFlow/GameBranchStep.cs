using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 分岐する際に使うGameStep
/// </summary>
[CreateAssetMenu(fileName = "GameBranchStep", menuName = "GameFlow/GameBranchStep")]
public class GameBranchStep : GameStep
{
    [Header("分岐条件と次のステップ")]
    [SerializeField]
    private List<BranchCondition> branchConditions;

    [System.Serializable]
    private struct BranchCondition
    {
        public float maxClearTime; // 条件のクリア時間
        public GameStep nextStep; // 条件を満たした場合の次のステップ
    }

    private bool isInitialized = false;

    public GameStep GetNextStepByClearTime(float clearTime)
    {
        InitBranchSteps();

        Debug.Log($"Game Clear Time: {clearTime}");
        // 最大クリアタイムが一番遅いものより、クリアタイムが遅い場合それを返す
        var bestBadEndCondition = branchConditions[branchConditions.Count - 1];
        if (bestBadEndCondition.maxClearTime <= clearTime)
        {
            Debug.Log($"Branching {bestBadEndCondition.nextStep.name}");
            return bestBadEndCondition.nextStep;
        }

        foreach (var condition in branchConditions)
        {
            if (condition.maxClearTime > clearTime)
            {
                Debug.Log($"Branching {condition.nextStep.name}");
                return condition.nextStep;
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

        if (branchConditions == null ||
            branchConditions.Count == 0)
        {
            Debug.LogError("Not set next step in branch step");
            return;
        }

        // クリア時間を昇順にソート
        branchConditions.Sort((x, y) => x.maxClearTime.CompareTo(y.maxClearTime));

        // 分岐内であるというフラグを立てる
        foreach (var branchCondition in branchConditions)
        {
            var step = branchCondition.nextStep;
            step.SetTrueIsInBranch();
        }

        isInitialized = true;
    }
}
