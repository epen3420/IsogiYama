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
}
