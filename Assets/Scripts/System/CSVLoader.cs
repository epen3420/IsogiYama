using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;

/// <summary>
/// CSVをListに格納
/// </summary>
namespace IsogiYama.System
{
    public class CSVLoader
    {

        /// <summary>
        /// Enumを利用したマッピングに対応するCSVの汎用読み込み
        /// </summary>
        /// <typeparam name="TEnum">Enum型（例: ScenarioFields, EnemyFields）</typeparam>
        /// <param name="csvFile">CSVファイル</param>
        /// <param name="dataName">データ名（省略時はファイル名）</param>
        /// <returns>読み込んだデータをまとめた CsvData&lt;TEnum&gt;</returns>
        public CsvData<TEnum> LoadCSV<TEnum>(TextAsset csvFile, string dataName = default)
            where TEnum : struct, Enum
        {
            // 結果を格納するコンテナ
            var result = new CsvData<TEnum>();
            result.DataName = string.IsNullOrEmpty(dataName) ? csvFile.name : dataName;

            // CSVの行を分割
            string[] lines = csvFile.text.Split('\n');
            if (lines.Length < 2)
                throw new Exception("CSVファイルが空か、ヘッダーがありません。");

            // ヘッダー行の解析
            string[] headers = lines[0].Split(',');
            var enumMapping = new Dictionary<int, TEnum>();

            for (int i = 0; i < headers.Length; i++)
            {
                string trimmed = headers[i].Trim();
                if (Enum.TryParse(trimmed, out TEnum field))
                    enumMapping[i] = field;
                else
                    throw new Exception($"Unknown header: {trimmed}");
            }

            // データ行を読み込み
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue; // 空行をスキップ

                string[] values = line.Split(',');
                if (values.Length != headers.Length)
                    throw new Exception($"行 {i + 1} の列数がヘッダーと一致しません。");

                var row = new LineData<TEnum>();

                // 各ヘッダーに対応する値を設定
                for (int j = 0; j < values.Length; j++)
                {
                    var str = values[j].Trim();
                    object parsed = ParseValue(str);
                    row[enumMapping[j]] = parsed;
                }

                result.Rows.Add(row);
            }

            Debug.Log($"Loaded CsvData<{typeof(TEnum).Name}> ({result.Rows.Count} 行) for '{result.DataName}'");
            return result;
        }

        // 煩雑な仕様をまとめてファイルを渡せばよいだけにしたメソッド
        public CsvData<ScenarioFields> ReadScenarioCSV(TextAsset file, string filename = default)
        {
            CsvData<ScenarioFields> scenarioData = new CsvData<ScenarioFields>();
            scenarioData = LoadCSV<ScenarioFields>(file);
            scenarioData.DataName = filename;

            return scenarioData;
        }

        // 型変換を処理するヘルパーメソッド
        private object ParseValue(string value)
        {
            if (int.TryParse(value, out int intValue)) return intValue;
            if (float.TryParse(value, out float floatValue)) return floatValue;
            if (bool.TryParse(value, out bool boolValue)) return boolValue;
            return value; // デフォルトは文字列として扱う
        }
    }
}