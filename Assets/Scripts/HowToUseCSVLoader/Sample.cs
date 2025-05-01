using UnityEngine;
using IsogiYama.System;

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
        // staticではないのでインスタンス化すること。
        CSVLoader CsvLoader = new CSVLoader();

        // 1. ScenarioFields を使って読み込み(CsvData<ScenarioFields>型) 第二引数は書かなくても良い、読み込んだCSV名を設定できる、
        // 複数のCSVをLoadCSVによって保持しておきたいときに使えるだろうが基本書かなくてよい。
        var scenarioData = CsvLoader.LoadCSV<ScenarioFields>(scenarioCsv, "MainScenario");
        Debug.Log($"[Scenario] '{scenarioData.DataName}' を読み込み完了。行数: {scenarioData.Rows.Count}");

        if (scenarioData.Rows.Count > 0)
        {
            // CsvData<ScenarioFields>型は、Rowsというリストを保持している。RowsはDictionaryの集合体
            // RowsにはGetメソッドが用意されていて、あらかじめ定義したEnumで列を指定したあと取得したい型に合わせてGet<T>のTを変える。(intとかfloatとか)

            var firstLine = scenarioData.Rows[0];
            string cmd = firstLine.Get<string>(ScenarioFields.Command);
            Debug.Log($"[Scenario] 先頭行の Command: {cmd}");
        }


        // 2. EnemyFields を使って読み込み
        var enemyData = CsvLoader.LoadCSV<EnemyFields>(enemyCsv, "EnemyStats");
        Debug.Log($"[Enemy] '{enemyData.DataName}' を読み込み完了。行数: {enemyData.Rows.Count}");

        foreach (var line in enemyData.Rows)
        {
            int id = line.Get<int>(EnemyFields.ID);
            string name = line.Get<string>(EnemyFields.Name);
            float hp = line.Get<int>(EnemyFields.HP);
            int attack = line.Get<int>(EnemyFields.Attack);

            Debug.Log($"[Enemy] ID:{id} / Name:{name} / HP:{hp} / Attack:{attack}");
        }
    }
}
