[System.Serializable]
public class BranchTransitionCondition
{
    public float minClearScore; // 条件の最低クリアスコア
    public GameStepNeedCSV nextStep; // 条件を満たした場合の次のステップ
}
