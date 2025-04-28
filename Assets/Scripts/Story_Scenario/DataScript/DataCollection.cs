using System.Collections.Generic;

/// <summary>
/// 一つのデータ、単位データの集合体
/// </summary>
[System.Serializable]
public class ScenarioData
{
    public string DataName;  // シナリオの名前
    public List<ScenarioLineData> scenarioDataList = new List<ScenarioLineData>();
}
