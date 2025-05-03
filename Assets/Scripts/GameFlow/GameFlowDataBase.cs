using UnityEngine;

/// <summary>
/// 一番元のゲームフローを管理するDataBase
/// このDataBaseを通してゲームフローを管理する
/// </summary>
[CreateAssetMenu(fileName = "GameFlowDataBase", menuName = "GameFlow/GameFlowDataBase")]
public class GameFlowDataBase : ScriptableObject
{
    public GameStepBase[] gameSteps;
}
