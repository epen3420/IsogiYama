using UnityEngine;

public abstract class GameStepBase : ScriptableObject
{
    [SerializeField] protected GameStepType stepType;
    public abstract GameStepType StepType { get; }
    public abstract TextAsset CsvFile { get; }
}
