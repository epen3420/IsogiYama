using System.Collections.Generic;

/// <summary>
/// Trie 木の1ノード。
/// 終端ノードは「ここまで入力されたときのひらがな消費数・ローマ字消費数」を持つ。
/// </summary>
public class TrieNode
{
    public readonly Dictionary<char, TrieNode> Children = new();

    // --- 終端情報 ---

    /// <summary>このノードでひとつのローマ字パターンが完成するか</summary>
    public bool IsTerminal;

    /// <summary>終端到達時点で消費したひらがな文字数（累積）</summary>
    public int HiraganaCount;

    /// <summary>終端到達時点で消費したローマ字文字数（累積）</summary>
    public int RomajiCount;

    /// <summary>
    /// 「っ」子音重ね入力で終端に達したとき、
    /// 次のセグメントの先頭として引き渡す文字。
    /// 通常は '\0'（引き渡しなし）。
    /// </summary>
    public char CarryOverChar;
}
