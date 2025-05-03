using UnityEngine;

[CreateAssetMenu(fileName = "GameStep", menuName = "GameFlow/GameStep")]
public class GameStep : GameStepBase
{
    [SerializeField] private TextAsset csvFile;

    public override GameStepType StepType => stepType;
    public override TextAsset CsvFile => csvFile;
}
