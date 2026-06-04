using System;
using System.Text;

/// <summary>
/// ひらがな列に対するタイピング判定クラス
/// </summary>
public class TypingJudder
{
    private TrieNode _root;
    private TrieNode _current;

    /// <summary>
    /// 遅延コミット中の中間終端ノード。
    /// </summary>
    private TrieNode _pendingTerminal;

    /// <summary>入力済みローマ字文字数（UI 表示用）</summary>
    public int TypedRomajiCount { get; private set; }

    /// <summary>入力済みひらがな文字数（UI 表示用）</summary>
    public int TypedHiraganaCount { get; private set; }

    public string FullJapanese { get; private set; }
    public string FullRomaji { get; private set; }

    /// <summary>
    /// セグメント完了ごとに発火。引数は現在の FullRomaji。
    /// ユーザーが別表記（shi ではなく si 等）を打った場合など、
    /// 表示上のローマ字列を更新したいタイミングで使用。
    /// </summary>
    public event Action<string> OnRomajiTextChanged;

    public TypingJudder(string hiragana)
    {
        FullJapanese = hiragana;
        FullRomaji = BuildDefaultRomaji(hiragana);

        _root = TrieBuilder.GetOrBuild(hiragana);
        _current = _root;

        TypedRomajiCount = 0;
        TypedHiraganaCount = 0;
        _pendingTerminal = null;
    }

    /// <summary>
    /// 入力判定。打鍵に対して Hit / Miss / Clear を返す。
    /// </summary>
    /// <param name="typedChar"></param>
    /// <returns></returns>
    public TypingState JudgeChar(char typedChar)
    {
#if UNITY_EDITOR
        if (CheatModeWindow.IsCheat)
        {
            if (TypedRomajiCount >= FullRomaji.Length)
                return TypingState.Clear;

            // どんな文字でも1文字進める
            TypedRomajiCount++;

            float progress = (float)TypedRomajiCount / FullRomaji.Length;
            TypedHiraganaCount = (int)Math.Round(FullJapanese.Length * progress);

            if (TypedRomajiCount >= FullRomaji.Length)
            {
                _current = null;
                _pendingTerminal = null;
                return TypingState.Clear;
            }

            return TypingState.Hit;
        }
#endif
        if (_current == null)
            return TypingState.Clear;

        char lower = char.ToLower(typedChar);

        if (!_current.Children.TryGetValue(lower, out TrieNode next))
        {
            // 現在ノードにマッチする子がない。
            // pending（ん shortcut 等）があれば確定して次セグメントの root へ fallback。
            if (_pendingTerminal != null)
            {
                var pending = _pendingTerminal;
                _pendingTerminal = null;

                TrieNode fallback = pending.NextRoot;
                if (fallback != null && fallback.Children.TryGetValue(lower, out next))
                {
                    // pending を確定
                    TypedHiraganaCount += pending.HiraganaCount;
                    TypedRomajiCount += pending.RomajiCount;
                    OnRomajiTextChanged?.Invoke(FullRomaji);

                    _current = next;
                    return next.IsTerminal ? CommitOrDefer(next) : TypingState.Hit;
                }
            }
            return TypingState.Miss;
        }

        _pendingTerminal = null;
        _current = next;

        if (!next.IsTerminal)
            return TypingState.Hit;

        return CommitOrDefer(next);
    }

    /// <summary>
    /// 終端ノードに到達したとき、即コミットするか遅延するかを判定する。
    /// </summary>
    private TypingState CommitOrDefer(TrieNode node)
    {
        if (node.Children.Count > 0 && node.NextRoot != null)
        {
            // 遅延：まだカウントを加算せず、次の打鍵を待つ
            _pendingTerminal = node;
            return TypingState.Hit;
        }

        // 即コミット
        TypedHiraganaCount += node.HiraganaCount;
        TypedRomajiCount += node.RomajiCount;
        OnRomajiTextChanged?.Invoke(FullRomaji);

        _current = node.NextRoot; // null なら全セグメント完了
        return _current == null ? TypingState.Clear : TypingState.Hit;
    }

    private static string BuildDefaultRomaji(string hiragana)
    {
        var sb = new StringBuilder();
        int i = 0;

        while (i < hiragana.Length)
        {
            bool found = false;

            if (hiragana[i] == 'っ')
            {
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
