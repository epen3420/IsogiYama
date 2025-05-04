using UnityEngine;

/// <summary>
/// ゲームフローのステップを管理するための抽象クラス
/// </summary>
[CreateAssetMenu(fileName = "GameStep", menuName = "GameFlow/GameStep")]
public class GameStep : GameStepBase
{
    [Header("このステップで使うCSVファイル")]
    [SerializeField]
    private TextAsset csvFile;

    public override GameStepType StepType => stepType;
    public override TextAsset CsvFile => csvFile;
}
