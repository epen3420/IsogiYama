using System.Collections.Generic;

/// <summary>
/// Trie 木の1ノード
/// </summary>
public class TrieNode
{
    public readonly Dictionary<char, TrieNode> Children = new();

    /// <summary>このノードでひとつのローマ字パターンが完成するか</summary>
    public bool IsTerminal;

    /// <summary>このセグメントで消費したひらがな文字数（差分）</summary>
    public int HiraganaCount;

    /// <summary>このセグメントで消費したローマ字文字数（差分）</summary>
    public int RomajiCount;
        
    /// <summary>
    /// 終端到達時に遷移する次セグメントの root ノード。
    /// 最終セグメントの終端では null。
    /// </summary>
    public TrieNode NextRoot;
}
