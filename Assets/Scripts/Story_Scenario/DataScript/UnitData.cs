using System;
using System.Collections.Generic;

/// <summary>
/// 読み込む際の1行、単位データ
/// </summary>
[System.Serializable]
public class ScenarioLineData
{
    private Dictionary<ScenarioFields, object> data = new Dictionary<ScenarioFields, object>();

    // インデクサでデータにアクセスする
    public object this[ScenarioFields field]
    {
        get { return data.ContainsKey(field) ? data[field] : null; }
        set { data[field] = value; }
    }

    // 型を安全に取得するためのジェネリックメソッド
    public T GetData<T>(ScenarioFields field)
    {
        if (data.ContainsKey(field) && data[field] is T)
        {
            return (T)data[field];
        }
        throw new InvalidCastException($"Field {field} cannot be cast to {typeof(T)}");
    }
}
