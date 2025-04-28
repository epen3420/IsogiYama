using System;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// シングルトンクラスだとどのシーン、クラスでも呼び出し放題になるからシーン内で共通の一つのインスタンスを使用するために
/// 初期化時にRegisterに登録し、使いたい場合はRegisterのGetを介してprivateな変数に各々明示的に保持することで
/// 多少なりとも制限できるし、処理を追えるという算段のクラス。OnDestroyにレジスターから削除するようにするべき。
/// 
/// コトレ
/// </summary>
public static class InstanceRegister
{
    private static Dictionary<Type, object> instances = new Dictionary<Type, object>();

    public static void Add<T> (T instance) where T : class
    {
        var type = typeof (T);
        if (!instances.ContainsKey(type))
        {
            instances.Add(type, instance);
        }
    }

    public static T Get<T>() where T : class
    {
        var type = typeof (T);
        return instances.ContainsKey(type) ? instances[type] as T : null;
    }

    public static void Remove<T>() where T : class
    {
        var type = typeof (T);
        if(instances.ContainsKey(type))
        {
            instances.Remove(type);
        }
    }
}
