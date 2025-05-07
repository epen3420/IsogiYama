using System.Collections.Generic;
using System;

/// <summary>
/// 任意の Enum をキーに、各セルの値を保持する汎用的な 1 行データ
/// </summary>
[Serializable]
public class LineData<TEnum> where TEnum : struct, Enum
{
    // 実際の値を保持するディクショナリ
    private Dictionary<TEnum, object> data = new Dictionary<TEnum, object>();

    // インデクサでのアクセス
    public object this[TEnum field]
    {
        get => data.ContainsKey(field) ? data[field] : null;
        set => data[field] = value;
    }

    // 型安全に取得するためのジェネリックメソッド
    public TValue Get<TValue>(TEnum field)
    {
        if (!data.TryGetValue(field, out var val))
            throw new KeyNotFoundException($"Field '{field}' does not exist.");

        if (val is TValue t)
            return t;

        try
        {
            return (TValue)Convert.ChangeType(val, typeof(TValue));
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Cannot convert field '{field}' value to {typeof(TValue).Name}.", ex);
        }
    }
}
