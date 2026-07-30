using UnityEngine;
using CSV4Unity;

// 扱いたいcsvのヘッダーをあらかじめここで定義しておく
// CSVのヘッダー順はこの宣言順でなくても良い(Attack, HP, ID, Nameみたいな順のヘッダーでも良い)
public enum EnemyFields
{
    ID,
    Name,
    HP,
    Attack
}

public class Sample : MonoBehaviour
{
    [Header("読み込む CSV ファイル")]
    [SerializeField] private TextAsset scenarioCsv;
    [SerializeField] private TextAsset enemyCsv;

    void Start()
    {
        // 1. ScenarioFields を使って読み込み。dataNameは省略可能。
        // 複数のCSVを識別して保持したい場合に指定する。
        var scenarioData = CSVLoader.LoadTable<ScenarioFields>(scenarioCsv, dataName: "MainScenario");
        Debug.Log($"[Scenario] '{scenarioData.Document.Name}' を読み込み完了。行数: {scenarioData.RowCount}");

        if (scenarioData.RowCount > 0)
        {
            var firstLine = scenarioData.Row(0);
            string cmd = firstLine[ScenarioFields.Command].Get<string>();
            Debug.Log($"[Scenario] 先頭行の Command: {cmd}");
        }


        // 2. EnemyFields を使って読み込み
        var enemyData = CSVLoader.LoadTable<EnemyFields>(enemyCsv, dataName: "EnemyStats");
        Debug.Log($"[Enemy] '{enemyData.Document.Name}' を読み込み完了。行数: {enemyData.RowCount}");

        for (int i = 0; i < enemyData.RowCount; i++)
        {
            CsvRow<EnemyFields> line = enemyData.Row(i);
            int id = line[EnemyFields.ID].Get<int>();
            string name = line[EnemyFields.Name].Get<string>();
            float hp = line[EnemyFields.HP].Get<float>();
            int attack = line[EnemyFields.Attack].Get<int>();

            Debug.Log($"[Enemy] ID:{id} / Name:{name} / HP:{hp} / Attack:{attack}");
        }
    }
}
