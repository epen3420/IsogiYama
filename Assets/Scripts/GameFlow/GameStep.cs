using UnityEngine;

/// <summary>
/// ゲームフローのステップを管理するための抽象クラス
/// </summary>
[CreateAssetMenu(fileName = "GameStep", menuName = "GameFlow/GameStep")]
public class GameStep : GameStepBase
{
    public override GameStepType StepType => stepType;
    public override TextAsset CsvFile => csvFile;
}
