using UnityEngine;

/// <summary>
/// ゲームフローのステップを管理するための抽象クラス
/// </summary>
public abstract class GameStepBase : ScriptableObject
{
    [Header("ステップの種類")]
    [SerializeField]
    protected GameStepType stepType;

    public abstract GameStepType StepType { get; } // ステップの種類
    public abstract TextAsset CsvFile { get; } // ステップで必要なCSV
}
