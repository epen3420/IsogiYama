using UnityEngine;

[CreateAssetMenu(fileName = "EndingGameStep", menuName = "GameFlow/EndingGameStep")]
public class EndingGameStep : GameStepNeedCSV
{
    [SerializeField]
    private EndingType endingType;
    public EndingType EndingType { get; private set; }
}
