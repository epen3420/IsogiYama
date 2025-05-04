using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameBranchStep", menuName = "GameFlow/GameBranchStep")]
public class GameBranchStep : GameStepBase
{
    [System.Serializable]
    private struct BranchCondition
    {
        public float maxClearTime; // 条件のクリア時間
        public GameStep nextStep; // 条件を満たした場合の次のステップ
    }

    [Header("分岐条件")]
    [SerializeField]
    private List<BranchCondition> branchConditions;

    public override GameStepType StepType => stepType;
    public override TextAsset CsvFile => null;
    public override bool HasBranch => true;

    public GameStep GetNextStepByClearTime(float clearTime)
    {
        // クリア時間でソート
        branchConditions.Sort((x, y) => x.maxClearTime.CompareTo(y.maxClearTime));

        foreach (var condition in branchConditions)
        {
            if (condition.maxClearTime >= clearTime)
            {
                return condition.nextStep;
            }
        }
        return null;
    }
}
