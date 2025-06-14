using UnityEngine;

[CreateAssetMenu(fileName = "GameStepNeedCSV", menuName = "GameFlow/GameStepNeedCSV")]
public class GameStepNeedCSV : GameStep
{
    [Header("CSVファイル")]
    [SerializeField]
    protected TextAsset csvFile;
    public TextAsset CsvFile => csvFile;
}
