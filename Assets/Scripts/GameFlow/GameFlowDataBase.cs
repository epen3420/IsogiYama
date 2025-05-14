using UnityEngine;

/// <summary>
/// 一番元のゲームフローのDataBase
/// このDataBaseを通してゲームフローを管理する
/// </summary>
[CreateAssetMenu(fileName = "GameFlowDataBase", menuName = "GameFlow/GameFlowDataBase")]
public class GameFlowDataBase : ScriptableObject
{
    public GameStep[] gameSteps;
}
