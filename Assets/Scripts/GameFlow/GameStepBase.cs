using UnityEngine;

public abstract class GameStepBase : ScriptableObject
{
    [Header("ステップの種類")]
    [SerializeField]
    protected GameStepType stepType;
    [Header("CSVファイル")]
    [SerializeField]
    protected TextAsset csvFile;

    public abstract GameStepType StepType { get; }
    public abstract TextAsset CsvFile { get; }
    // 分岐が必要な場合のみoverrideで使う
    public virtual bool HasBranch => false;
}
