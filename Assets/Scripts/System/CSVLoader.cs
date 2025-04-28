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

        void LoadCSV(TextAsset csvFile, string dataName = default)
        {
            ScenarioData scenario = new ScenarioData();
            if(dataName == null)
            {
                scenario.DataName = csvFile.name;
            }else scenario.DataName = dataName;

            string[] lines = csvFile.text.Split('\n');
            string[] headers = lines[0].Split(','); // 1行目をヘッダーとして扱う

            //Debug.Log("lines length: " + lines.Length);

            // Enumとヘッダーが一致するか確認する
            var fieldMap = new Dictionary<string, ScenarioFields>();
            foreach (var header in headers)
            {
                if (Enum.TryParse<ScenarioFields>(header.Trim(), out var field))
                {
                    fieldMap[header.Trim()] = field;
                }
                else
                {
                    throw new Exception($"Unknown header: {header}");
                }
            }

            // CSVデータの読み込み
            for (int i = 1; i < (lines.Length - 1); i++) // 2行目以降をデータとして扱う
            {
                Debug.Log("i: " + i);

                string[] values = lines[i].Split(',');
                Debug.Log("values length: " + values.Length);

                // 例外処理
                if (values.Length != headers.Length)
                {
                    throw new Exception($"Row {i + 1} has an incorrect number of columns.");
                }

                // UnitData
                ScenarioLineData scenarioData = new ScenarioLineData();

                // 各列を対応するフィールドに格納
                for (int j = 0; j < headers.Length; j++)
                {
                    //Debug.Log("j: " + j);

                    ScenarioFields field = fieldMap[headers[j].Trim()];
                    string value = values[j].Trim();

                    // 値を適切な型に変換して格納
                    if (bool.TryParse(value, out bool boolValue))
                    {
                        scenarioData[field] = boolValue;
                    }
                    else if (int.TryParse(value, out int intValue))
                    {
                        scenarioData[field] = intValue;
                    }
                    else if (float.TryParse(value, out float floatValue))
                    {
                        scenarioData[field] = floatValue;
                    }
                    else
                    {
                        scenarioData[field] = value; // 文字列として格納
                    }
                }

                // シナリオデータを追加
                scenario.scenarioDataList.Add(scenarioData);
            }
        }

        /// <summary>
        /// Enumを利用したマッピングに対応するCSVの読み込み
        /// </summary>
        /// <typeparam name="T">読み込み対象のデータ型（例: ScenarioLineData）</typeparam>
        /// <typeparam name="TEnum">Enum型（例: ScenarioFields）</typeparam>
        /// <param name="csvFile">CSVファイル</param>
        /// <returns>読み込んだデータのリスト</returns>
        public List<T> GeneralLoadCSV<T, TEnum>(TextAsset csvFile) where T : new() where TEnum : struct, Enum
        {
            // 結果を格納するリスト
            List<T> resultList = new List<T>();

            // CSVの行を分割
            string[] lines = csvFile.text.Split('\n');
            if (lines.Length < 2) throw new Exception("CSVファイルが空か、ヘッダーがありません。");

            // ヘッダー行の解析
            string[] headers = lines[0].Split(',');
            Dictionary<string, TEnum> enumMapping = new Dictionary<string, TEnum>();

            foreach (string header in headers)
            {
                string trimmedHeader = header.Trim();
                if (Enum.TryParse(trimmedHeader, out TEnum field))
                {
                    enumMapping[trimmedHeader] = field;
                }
                else
                {
                    throw new Exception($"Unknown header: {trimmedHeader}");
                }
            }

            // データ行を読み込み
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue; // 空行をスキップ

                string[] values = lines[i].Split(',');
                if (values.Length != headers.Length)
                {
                    throw new Exception($"行 {i + 1} の列数がヘッダーと一致しません。");
                }

                // 新しいデータ行のインスタンスを作成
                T lineData = new T();
                FieldInfo dataField = typeof(T).GetField("data", BindingFlags.NonPublic | BindingFlags.Instance);
                if (dataField == null)
                {
                    throw new Exception($"型 {typeof(T).Name} に 'data' フィールドが見つかりません。");
                }

                var dataDictionary = dataField.GetValue(lineData) as IDictionary<TEnum, object>;
                if (dataDictionary == null)
                {
                    throw new Exception($"'data' フィールドが Dictionary<{typeof(TEnum).Name}, object> ではありません。");
                }

                // 各ヘッダーに対応する値を設定
                for (int j = 0; j < headers.Length; j++)
                {
                    string header = headers[j].Trim();
                    string value = values[j].Trim();

                    if (enumMapping.TryGetValue(header, out TEnum field))
                    {
                        dataDictionary[field] = ParseValue(value);
                    }
                }

                resultList.Add(lineData);
            }

            Debug.Log($"型 {typeof(T).Name}の要素を型 {typeof(TEnum).Name}としてCSVを読み込み完了");
            return resultList;
        }


        // 煩雑な仕様をまとめてファイルを渡せばよいだけにしたメソッド
        public ScenarioData ReadScenarioCSV(TextAsset file, string filename = default)
        {
            ScenarioData scenarioData = new ScenarioData();
            scenarioData.scenarioDataList = GeneralLoadCSV<ScenarioLineData, ScenarioFields>(file);
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