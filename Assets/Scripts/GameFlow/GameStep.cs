using UnityEngine;

/// <summary>
/// ゲームフローのステップを管理するためのクラス
/// </summary>
[CreateAssetMenu(fileName = "GameStep", menuName = "GameFlow/GameStep")]
public class GameStep : ScriptableObject
{
    [Header("ステップの種類")]
    [SerializeField]
    protected GameStepType stepType;
    public GameStepType StepType => stepType;

    [Header("CSVファイル")]
    [SerializeField]
    protected TextAsset csvFile;
    public TextAsset CsvFile => csvFile;

    // 分岐中かどうか
    public bool IsInBranch => isInBranch;
    private bool isInBranch = false;
    public void SetTrueIsInBranch()
    {
        isInBranch = true;
    }
}
