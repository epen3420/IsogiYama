[System.Serializable]
public class BranchTransitionCondition
{
    public float maxClearTime; // 条件のクリア時間
    public GameStepNeedCSV nextStep; // 条件を満たした場合の次のステップ
}
