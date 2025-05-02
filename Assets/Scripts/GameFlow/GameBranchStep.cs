using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameBranchStep", menuName = "GameFlow/GameBranchStep")]
public class GameBranchStep : GameStepBase
{
    [System.Serializable]
    private struct BranchCondition
    {
        public float maxClearTime;
        public GameStep nextStep;
    }
    [SerializeField] private List<BranchCondition> branchConditions = new List<BranchCondition>();

    public override GameStepType StepType => GameStepType.Branch;
    public override TextAsset CsvFile => null;

    public GameStep GetNextStepByClearTime(float clearTime)
    {
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
