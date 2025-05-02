using UnityEngine;

[CreateAssetMenu(fileName = "GameFlowDataBase", menuName = "GameFlow/GameFlowDataBase")]
public class GameFlowDataBase : ScriptableObject
{
    public GameStep[] gameSteps;
}
