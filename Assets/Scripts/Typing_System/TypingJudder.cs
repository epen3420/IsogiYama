using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// ひらがな列に対するタイピング判定クラス
/// </summary>
public class TypingJudder
{
    private TrieNode _root;
    private TrieNode _current;
    private readonly StringBuilder _committedRomaji = new();
    private readonly StringBuilder _currentRomaji = new();
    private readonly StringBuilder _fullRomajiBuilder = new();
    private readonly Dictionary<int, string> _defaultRomajiSuffixCache = new();

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
    /// FullRomaji が変化したときに発火。引数は現在の FullRomaji。
    /// ユーザーが別表記（shi ではなく si 等）を打った場合など、
    /// 表示上のローマ字列を更新したいタイミングで使用。
    /// </summary>
    public event Action<string> OnRomajiTextChanged;

    public TypingJudder(string hiragana)
    {
        FullJapanese = hiragana;
        FullRomaji = BuildDefaultRomaji(hiragana);
        _defaultRomajiSuffixCache[0] = FullRomaji;

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

                TrieNode fallback = pending.NextRoot;
                if (fallback != null && fallback.Children.TryGetValue(lower, out next))
                {
                    // pending を確定
                    CommitPendingTerminal(pending);

                    _currentRomaji.Append(lower);
                    _current = next;
                    RefreshRomajiText();
                    return next.IsTerminal ? CommitOrDefer(next) : TypingState.Hit;
                }
            }
            return TypingState.Miss;
        }

        _pendingTerminal = null;
        _currentRomaji.Append(lower);
        _current = next;
        RefreshRomajiText();

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
            RefreshRomajiText();
            return TypingState.Hit;
        }

        // 即コミット
        CommitTerminal(node);
        return _current == null ? TypingState.Clear : TypingState.Hit;
    }

    private void CommitPendingTerminal(TrieNode node)
    {
        TypedHiraganaCount += node.HiraganaCount;
        _committedRomaji.Append(_currentRomaji);
        _currentRomaji.Clear();
        _pendingTerminal = null;
    }

    private void CommitTerminal(TrieNode node)
    {
        CommitPendingTerminal(node);
        _current = node.NextRoot; // null なら全セグメント完了
        RefreshRomajiText();
    }

    private void RefreshRomajiText()
    {
        TypedRomajiCount = _committedRomaji.Length + _currentRomaji.Length;

        if (_current == null)
        {
            SetFullRomaji(_committedRomaji.ToString());
            return;
        }

        if (_currentRomaji.Length == 0)
        {
            SetFullRomaji(BuildFullRomaji(GetDefaultRomajiFrom(TypedHiraganaCount)));
            return;
        }

        if (TryGetDefaultCompletion(_current, out string completion, out TrieNode terminal))
        {
            int nextHiraganaIndex = TypedHiraganaCount + terminal.HiraganaCount;
            SetFullRomaji(BuildFullRomaji(_currentRomaji, completion, GetDefaultRomajiFrom(nextHiraganaIndex)));
        }
        else
        {
            SetFullRomaji(BuildFullRomaji(_currentRomaji));
        }
    }

    private void SetFullRomaji(string newFullRomaji)
    {
        if (FullRomaji == newFullRomaji) return;

        FullRomaji = newFullRomaji;
        OnRomajiTextChanged?.Invoke(FullRomaji);
    }

    private string BuildFullRomaji(string suffix)
    {
        _fullRomajiBuilder.Clear();
        AppendStringBuilder(_fullRomajiBuilder, _committedRomaji);
        _fullRomajiBuilder.Append(suffix);
        return _fullRomajiBuilder.ToString();
    }

    private string BuildFullRomaji(StringBuilder currentRomaji)
    {
        _fullRomajiBuilder.Clear();
        AppendStringBuilder(_fullRomajiBuilder, _committedRomaji);
        AppendStringBuilder(_fullRomajiBuilder, currentRomaji);
        return _fullRomajiBuilder.ToString();
    }

    private string BuildFullRomaji(StringBuilder currentRomaji, string completion, string suffix)
    {
        _fullRomajiBuilder.Clear();
        AppendStringBuilder(_fullRomajiBuilder, _committedRomaji);
        AppendStringBuilder(_fullRomajiBuilder, currentRomaji);
        _fullRomajiBuilder.Append(completion);
        _fullRomajiBuilder.Append(suffix);
        return _fullRomajiBuilder.ToString();
    }

    private static void AppendStringBuilder(StringBuilder destination, StringBuilder source)
    {
        for (int i = 0; i < source.Length; i++)
            destination.Append(source[i]);
    }

    private string GetDefaultRomajiFrom(int hiraganaIndex)
    {
        if (hiraganaIndex >= FullJapanese.Length) return "";

        if (!_defaultRomajiSuffixCache.TryGetValue(hiraganaIndex, out string romaji))
        {
            romaji = BuildDefaultRomaji(FullJapanese, hiraganaIndex);
            _defaultRomajiSuffixCache[hiraganaIndex] = romaji;
        }

        return romaji;
    }

    private static bool TryGetDefaultCompletion(TrieNode node, out string completion, out TrieNode terminal)
    {
        if (node.IsTerminal)
        {
            completion = "";
            terminal = node;
            return true;
        }

        if (node.DefaultTerminal != null)
        {
            completion = node.DefaultCompletion;
            terminal = node.DefaultTerminal;
            return true;
        }

        completion = "";
        terminal = null;
        return false;
    }

    private static string BuildDefaultRomaji(string hiragana)
    {
        return BuildDefaultRomaji(hiragana, 0);
    }

    private static string BuildDefaultRomaji(string hiragana, int startIndex)
    {
        var sb = new StringBuilder();
        int i = startIndex;

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
                    if ("aiueon".IndexOf(c) < 0 && c != 'y') carry = c;
                    break;
                }
                sb.Append(carry != '\0' ? carry : TrieBuilder.RomaMap["っ"][0]);
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
