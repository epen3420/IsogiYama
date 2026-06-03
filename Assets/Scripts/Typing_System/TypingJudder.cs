using System;
using System.Text;

/// <summary>
/// ひらがな列に対するタイピング判定クラス。
/// 状態は CurrentNode ポインタと2つのカウンタのみ。
/// 出題ごとの new TrieNode / new Dictionary は一切発生しない。
/// </summary>
public class TypingJudder
{
    // -------------------------------------------------------------------------
    // 状態（出題ごとにリセット）
    // -------------------------------------------------------------------------

    private TrieNode _root;
    private TrieNode _current;

    /// <summary>入力済みローマ字文字数（UI 表示用）</summary>
    public int TypedRomajiCount { get; private set; }

    /// <summary>入力済みひらがな文字数（UI 表示用）</summary>
    public int TypedHiraganaCount { get; private set; }

    // -------------------------------------------------------------------------
    // 表示用テキスト
    // -------------------------------------------------------------------------

    /// <summary>現在の問題のひらがな全体（UI 表示用）</summary>
    public string FullJapanese { get; private set; }

    /// <summary>
    /// 現在の問題の表示用ローマ字列（デフォルト表記）。
    /// ユーザーが別表記（shi ではなく si 等）を打った場合も
    /// このクラスでは変更しない。変更が必要なら OnRomajiTextChanged を利用。
    /// </summary>
    public string FullRomaji { get; private set; }

    /// <summary>
    /// セグメント完了ごとに発火。引数は現在の FullRomaji。
    /// 表示上のローマ字列を更新したいタイミングで使用。
    /// </summary>
    public event Action<string> OnRomajiTextChanged;

    // -------------------------------------------------------------------------
    // コンストラクタ
    // -------------------------------------------------------------------------

    public TypingJudder(string hiragana)
    {
        FullJapanese = hiragana;
        FullRomaji = BuildDefaultRomaji(hiragana);

        _root = TrieBuilder.GetOrBuild(hiragana);
        _current = _root;

        TypedRomajiCount = 0;
        TypedHiraganaCount = 0;
    }

    // -------------------------------------------------------------------------
    // 入力判定
    // -------------------------------------------------------------------------

    public TypingState JudgeChar(char typedChar)
    {
#if UNITY_EDITOR
        if (CheatModeWindow.IsCheat)
        {
            _current = null;
            TypedRomajiCount = FullRomaji.Length;
            TypedHiraganaCount = FullJapanese.Length;
            return TypingState.Clear;
        }
#endif
        if (_current == null)
            return TypingState.Clear;

        char lower = char.ToLower(typedChar);

        if (!_current.Children.TryGetValue(lower, out TrieNode next))
            return TypingState.Miss;

        _current = next;

        if (!next.IsTerminal)
            return TypingState.Hit;

        // --- 終端到達：カウンタ更新 ---
        TypedRomajiCount = next.RomajiCount;
        TypedHiraganaCount = next.HiraganaCount;

        OnRomajiTextChanged?.Invoke(FullRomaji);

        if (TypedHiraganaCount >= FullJapanese.Length)
        {
            _current = null;
            return TypingState.Clear;
        }

        return TypingState.Hit;
    }

    // -------------------------------------------------------------------------
    // デフォルト表示用ローマ字列の生成
    // -------------------------------------------------------------------------

    /// <summary>
    /// ひらがな列から表示用のデフォルトローマ字列を生成する。
    /// 「っ」は次の子音重ね表記を使用する。
    /// </summary>
    private static string BuildDefaultRomaji(string hiragana)
    {
        var sb = new StringBuilder();
        int i = 0;

        while (i < hiragana.Length)
        {
            bool found = false;

            if (hiragana[i] == 'っ')
            {
                // 次の先頭子音を先読みして重ね表記
                char carry = '\0';
                for (int len = Math.Min(3, hiragana.Length - (i + 1)); len >= 1; len--)
                {
                    string sub = hiragana.Substring(i + 1, len);
                    if (!TrieBuilder.RomaMap.TryGetValue(sub, out string[] r)) continue;
                    char c = r[0][0];
                    if (!"aiueon".Contains(c) && c != 'y') carry = c;
                    break;
                }
                sb.Append(carry != '\0' ? carry : TrieBuilder.RomaMap["っ"][0][0]);
                i++;
                found = true;
            }

            if (!found)
            {
                for (int len = Math.Min(3, hiragana.Length - i); len >= 1; len--)
                {
                    string sub = hiragana.Substring(i, len);
                    if (!TrieBuilder.RomaMap.TryGetValue(sub, out string[] r)) continue;
                    sb.Append(r[0]);
                    i += len;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                sb.Append(hiragana[i]);
                i++;
            }
        }

        return sb.ToString();
    }
}
