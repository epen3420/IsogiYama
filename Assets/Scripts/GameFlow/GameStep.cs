using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStepDataBase", menuName = "GameFlow/GameStepDataBase")]
public class GameStep : ScriptableObject
{
    public GameStepType stepType;
    public TextAsset csvFile;

    public GameStep GetNextStepByClearTime(float clearTime)
    {
        if (stepType != GameStepType.Branch)
            return null;

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

    public List<BranchCondition> branchConditions;
}

public enum GameStepType
{
    None = -1,
    Story = 0,
    Typing = 1,
    Branch = 2
}

[System.Serializable]
public class BranchCondition
{
    public float maxClearTime;
    public GameStep nextStep;
}
