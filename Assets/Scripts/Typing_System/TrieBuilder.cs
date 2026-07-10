using System.Collections.Generic;
using System;

/// <summary>
/// Trie 木を構築。
/// </summary>
public static class TrieBuilder
{
    // ひらがな列全体をキーとする先頭 root のキャッシュ
    private static readonly Dictionary<string, TrieNode> RootCache = new();

    // セグメント単位のキャッシュ (セグメントひらがな, 次セグメントroot) → セグメントroot
    private static readonly Dictionary<(string, TrieNode), TrieNode> SegmentCache = new();

    // ひらがなローマ字 対応マップ
    public static readonly Dictionary<string, string[]> RomaMap = new ()
    {
        {"あ", new[] {"a"}},
        {"い", new[] {"i", "yi"}},
        {"う", new[] {"u", "wu", "whu"}},
        {"え", new[] {"e"}},
        {"お", new[] {"o"}},

        {"か", new[] {"ka", "ca"}},
        {"き", new[] {"ki"}},
        {"く", new[] {"ku", "cu"}},
        {"け", new[] {"ke"}},
        {"こ", new[] {"ko", "co"}},

        {"さ", new[] {"sa"}},
        {"し", new[] {"si", "shi", "ci"}},
        {"す", new[] {"su"}},
        {"せ", new[] {"se", "ce"}},
        {"そ", new[] {"so"}},

        {"た", new[] {"ta"}},
        {"ち", new[] {"ti", "chi"}},
        {"つ", new[] {"tu", "tsu"}},
        {"て", new[] {"te"}},
        {"と", new[] {"to"}},

        {"な", new[] {"na"}},
        {"に", new[] {"ni"}},
        {"ぬ", new[] {"nu"}},
        {"ね", new[] {"ne"}},
        {"の", new[] {"no"}},

        {"は", new[] {"ha"}},
        {"ひ", new[] {"hi"}},
        {"ふ", new[] {"hu", "fu"}},
        {"へ", new[] {"he"}},
        {"ほ", new[] {"ho"}},

        {"ま", new[] {"ma"}},
        {"み", new[] {"mi"}},
        {"む", new[] {"mu"}},
        {"め", new[] {"me"}},
        {"も", new[] {"mo"}},

        {"や", new[] {"ya"}},
        {"ゆ", new[] {"yu"}},
        {"よ", new[] {"yo"}},

        {"ら", new[] {"ra"}},
        {"り", new[] {"ri"}},
        {"る", new[] {"ru"}},
        {"れ", new[] {"re"}},
        {"ろ", new[] {"ro"}},

        {"わ", new[] {"wa"}},
        {"を", new[] {"wo"}},
        {"ん", new[] {"nn", "xn"}},

        {"が", new[] {"ga"}},
        {"ぎ", new[] {"gi"}},
        {"ぐ", new[] {"gu"}},
        {"げ", new[] {"ge"}},
        {"ご", new[] {"go"}},

        {"ざ", new[] {"za"}},
        {"じ", new[] {"zi", "ji"}},
        {"ず", new[] {"zu"}},
        {"ぜ", new[] {"ze"}},
        {"ぞ", new[] {"zo"}},

        {"だ", new[] {"da"}},
        {"ぢ", new[] {"di"}},
        {"づ", new[] {"du"}},
        {"で", new[] {"de"}},
        {"ど", new[] {"do"}},

        {"ば", new[] {"ba"}},
        {"び", new[] {"bi"}},
        {"ぶ", new[] {"bu"}},
        {"べ", new[] {"be"}},
        {"ぼ", new[] {"bo"}},

        {"ぱ", new[] {"pa"}},
        {"ぴ", new[] {"pi"}},
        {"ぷ", new[] {"pu"}},
        {"ぺ", new[] {"pe"}},
        {"ぽ", new[] {"po"}},

        // 小さい仮名
        {"ぁ", new[] {"xa", "la"}},
        {"ぃ", new[] {"xi", "li"}},
        {"ぅ", new[] {"xu", "lu"}},
        {"ぇ", new[] {"xe", "le"}},
        {"ぉ", new[] {"xo", "lo"}},
        {"ゃ", new[] {"xya", "lya"}},
        {"ゅ", new[] {"xyu", "lyu"}},
        {"ょ", new[] {"xyo", "lyo"}},
        {"っ", new[] {"ltu", "xtu", "xtsu", "ltsu"}},

        // 複合音
        {"きゃ", new[] {"kya"}},
        {"きぃ", new[] {"kyi"}},
        {"きゅ", new[] {"kyu"}},
        {"きぇ", new[] {"kye"}},
        {"きょ", new[] {"kyo"}},

        {"くぁ", new[] {"qa"}},
        {"くぃ", new[] {"qi"}},
        {"くぅ", new[] {"qwu"}},
        {"くぇ", new[] {"qe"}},
        {"くぉ", new[] {"qo"}},

        {"ぎゃ", new[] {"gya"}},
        {"ぎぃ", new[] {"gyi"}},
        {"ぎゅ", new[] {"gyu"}},
        {"ぎぇ", new[] {"gye"}},
        {"ぎょ", new[] {"gyo"}},

        {"ぐぁ", new[] {"gwa"}},
        {"ぐぃ", new[] {"gwi"}},
        {"ぐぅ", new[] {"gwu"}},
        {"ぐぇ", new[] {"gwe"}},
        {"ぐぉ", new[] {"gwo"}},

        {"しゃ", new[] {"sya", "sha" }},
        {"しぃ", new[] {"syi"}},
        {"しゅ", new[] {"syu", "shu" }},
        {"しぇ", new[] {"she", "sye" }},
        {"しょ", new[] {"sho", "syo" }},

        {"すぁ", new[] {"swa"}},
        {"すぃ", new[] {"swi"}},
        {"すぅ", new[] {"swu"}},
        {"すぇ", new[] {"swe"}},
        {"すぉ", new[] {"swo"}},

        {"じゃ", new[] {"ja", "zya"}},
        {"じぃ", new[] {"zyi"}},
        {"じゅ", new[] {"ju", "zyu"}},
        {"じぇ", new[] {"je", "zye"}},
        {"じょ", new[] {"jo", "zyo"}},

        {"ちゃ", new[] {"tya", "cha", "cya"}},
        {"ちぃ", new[] {"tyi", "cyi"}},
        {"ちゅ", new[] {"tyu", "chu", "cyu"}},
        {"ちぇ", new[] {"tye", "che", "cye"}},
        {"ちょ", new[] {"tyo", "cho", "cyo"}},

        {"つぁ", new[] {"tsa"}},
        {"つぃ", new[] {"tsi"}},
        {"つぇ", new[] {"tse"}},
        {"つぉ", new[] {"tso"}},

        {"てゃ", new[] {"tha"}},
        {"てぃ", new[] {"thi"}},
        {"てゅ", new[] {"thu"}},
        {"てぇ", new[] {"the"}},
        {"てょ", new[] {"tho"}},

        {"とぁ", new[] {"twa"}},
        {"とぃ", new[] {"twi"}},
        {"とぅ", new[] {"twu"}},
        {"とぇ", new[] {"twe"}},
        {"とぉ", new[] {"two"}},

        {"ぢゃ", new[] {"dya"}},
        {"ぢぃ", new[] {"dyi"}},
        {"ぢゅ", new[] {"dyu"}},
        {"ぢぇ", new[] {"dye"}},
        {"ぢょ", new[] {"dyo"}},

        {"でゃ", new[] {"dha"}},
        {"でぃ", new[] {"dhi"}},
        {"でゅ", new[] {"dhu"}},
        {"でぇ", new[] {"dhe"}},
        {"でょ", new[] {"dho"}},

        {"どぁ", new[] {"dwa"}},
        {"どぃ", new[] {"dwi"}},
        {"どぅ", new[] {"dwu"}},
        {"どぇ", new[] {"dwe"}},
        {"どぉ", new[] {"dwo"}},

        {"にゃ", new[] {"nya"}},
        {"にぃ", new[] {"nyi"}},
        {"にゅ", new[] {"nyu"}},
        {"にぇ", new[] {"nye"}},
        {"にょ", new[] {"nyo"}},

        {"ひゃ", new[] {"hya"}},
        {"ひぃ", new[] {"hyi"}},
        {"ひゅ", new[] {"hyu"}},
        {"ひぇ", new[] {"hye"}},
        {"ひょ", new[] {"hyo"}},

        {"ふぁ", new[] {"fa"}},
        {"ふぃ", new[] {"fi"}},
        {"ふぅ", new[] {"fwu"}},
        {"ふぇ", new[] {"fe"}},
        {"ふぉ", new[] {"fo"}},

        {"びゃ", new[] {"bya"}},
        {"びぃ", new[] {"byi"}},
        {"びゅ", new[] {"byu"}},
        {"びぇ", new[] {"bye"}},
        {"びょ", new[] {"byo"}},

        {"ぴゃ", new[] {"pya"}},
        {"ぴぃ", new[] {"pyi"}},
        {"ぴゅ", new[] {"pyu"}},
        {"ぴぇ", new[] {"pye"}},
        {"ぴょ", new[] {"pyo"}},

        {"みゃ", new[] {"mya"}},
        {"みぃ", new[] {"myi"}},
        {"みゅ", new[] {"myu"}},
        {"みぇ", new[] {"mye"}},
        {"みょ", new[] {"myo"}},

        {"りゃ", new[] {"rya"}},
        {"りぃ", new[] {"ryi"}},
        {"りゅ", new[] {"ryu"}},
        {"りぇ", new[] {"rye"}},
        {"りょ", new[] {"ryo"}},

        {"うぁ", new[] {"wha"}},
        {"うぃ", new[] {"whi"}},
        {"うぇ", new[] {"whe"}},
        {"うぉ", new[] {"who"}}
    };

    /// <summary>
    /// ひらがな列に対応する先頭セグメントの Trie root を返す
    /// </summary>
    public static TrieNode GetOrBuild(string hiragana)
    {
        if (RootCache.TryGetValue(hiragana, out TrieNode cached))
            return cached;

        var segments = ParseSegments(hiragana);

        // 末尾から逆順に構築して nextRoot を繋いでいく
        TrieNode nextRoot = null;
        for (int i = segments.Count - 1; i >= 0; i--)
            nextRoot = GetOrBuildSegmentRoot(segments[i], nextRoot);

        RootCache[hiragana] = nextRoot; // nextRoot は先頭セグメントの root
        return nextRoot;
    }

    private static TrieNode GetOrBuildSegmentRoot(Segment seg, TrieNode nextRoot)
    {
        var key = (seg.Hiragana, nextRoot);
        if (SegmentCache.TryGetValue(key, out TrieNode cached))
            return cached;

        TrieNode root = BuildSegmentTrie(seg, nextRoot);
        SegmentCache[key] = root;
        return root;
    }

    /// <summary>
    /// セグメント1つ分の Trie を構築する。
    /// 各終端ノードの NextRoot に nextRoot を設定する。
    /// HiraganaCount / RomajiCount はこのセグメント単体の差分値。
    /// </summary>
    private static TrieNode BuildSegmentTrie(Segment seg, TrieNode nextRoot)
    {
        var root = new TrieNode();
        int hiraLen = seg.Hiragana.Length;

        // 通常ローマ字候補
        foreach (string roma in seg.Romajis)
            InsertPath(root, roma, hiraLen, nextRoot);

        // 複合音の分割入力候補（例: きょ → ki+xyo）
        if (seg.SplitRomajis != null)
            foreach (string roma in seg.SplitRomajis)
                InsertPath(root, roma, hiraLen, nextRoot);

        // 「っ」子音重ね 子音1文字で っ 完了、NextRoot で次セグに飛ぶ
        if (seg.ConsonantCarry != '\0')
            InsertPath(root, seg.ConsonantCarry.ToString(), hiraLen, nextRoot);

        // 「ん」省略入力：'n' 1文字で ん 完了（次が子音の場合のみ許容）
        if (seg.NShortcut != '\0')
            InsertPath(root, "n", hiraLen, nextRoot);

        return root;
    }

    /// <summary>
    /// path を node 以下に挿入し、末尾ノードを終端として記録する。
    /// すでに終端として設定済みのノードは上書きしない
    /// </summary>
    private static void InsertPath(TrieNode node, string path, int hiraCount, TrieNode nextRoot)
    {
        TrieNode cur = node;
        var pathNodes = new List<TrieNode>(path.Length);
        foreach (char c in path)
        {
            if (!cur.Children.TryGetValue(c, out TrieNode child))
            {
                child = new TrieNode();
                cur.Children[c] = child;
            }
            cur = child;
            pathNodes.Add(cur);
        }

        if (!cur.IsTerminal)
        {
            cur.IsTerminal = true;
            cur.HiraganaCount = hiraCount;
            cur.RomajiCount = path.Length;
            cur.NextRoot = nextRoot;
        }

        for (int i = 0; i < pathNodes.Count; i++)
        {
            TrieNode pathNode = pathNodes[i];
            if (pathNode.DefaultTerminal != null) continue;

            pathNode.DefaultCompletion = path.Substring(i + 1);
            pathNode.DefaultTerminal = cur;
        }
    }

    private readonly struct Segment
    {
        public readonly string Hiragana;
        public readonly string[] Romajis;
        public readonly string[] SplitRomajis;
        public readonly char ConsonantCarry;
        public readonly char NShortcut;

        public Segment(string hiragana, string[] romajis,
                       string[] splitRomajis = null,
                       char consonantCarry = '\0',
                       char nShortcut = '\0')
        {
            Hiragana = hiragana;
            Romajis = romajis;
            SplitRomajis = splitRomajis;
            ConsonantCarry = consonantCarry;
            NShortcut = nShortcut;
        }
    }

    private static List<Segment> ParseSegments(string hiragana)
    {
        var result = new List<Segment>();
        int i = 0;

        while (i < hiragana.Length)
        {
            bool found = false;

            for (int len = Math.Min(3, hiragana.Length - i); len >= 1; len--)
            {
                string sub = hiragana.Substring(i, len);
                if (!RomaMap.TryGetValue(sub, out string[] romajis))
                    continue;

                if (sub == "っ")
                {
                    char carry = GetConsonantCarry(hiragana, i + 1);
                    result.Add(new Segment(sub, romajis, consonantCarry: carry));
                }
                else if (sub == "ん")
                {
                    char shortcut = GetNShortcut(hiragana, i + 1);
                    result.Add(new Segment(sub, romajis, nShortcut: shortcut));
                }
                else if (sub.Length > 1)
                {
                    string[] split = BuildSplitRomajis(sub);
                    result.Add(new Segment(sub, romajis, splitRomajis: split));
                }
                else
                {
                    result.Add(new Segment(sub, romajis));
                }

                i += len;
                found = true;
                break;
            }

            if (!found)
            {
                result.Add(new Segment(hiragana[i].ToString(),
                                       new[] { hiragana[i].ToString() }));
                i++;
            }
        }

        return result;
    }

    private static string[] BuildSplitRomajis(string combinedHiragana)
    {
        string first = combinedHiragana.Substring(0, 1);
        string rest = combinedHiragana.Substring(1);

        if (!RomaMap.TryGetValue(first, out string[] fr)
         || !RomaMap.TryGetValue(rest, out string[] rr))
            return null;

        var list = new List<string>(fr.Length * rr.Length);
        foreach (string f in fr)
            foreach (string r in rr)
                list.Add(f + r);

        return list.ToArray();
    }

    private static char GetConsonantCarry(string hiragana, int from)
    {
        if (from >= hiragana.Length) return '\0';
        for (int len = Math.Min(3, hiragana.Length - from); len >= 1; len--)
        {
            string sub = hiragana.Substring(from, len);
            if (!RomaMap.TryGetValue(sub, out string[] r)) continue;
            char c = r[0][0];
            return ("aiueon".IndexOf(c) < 0 && c != 'y') ? c : '\0';
        }
        return '\0';
    }

    private static char GetNShortcut(string hiragana, int from)
    {
        if (from >= hiragana.Length) return '\0';
        for (int len = Math.Min(3, hiragana.Length - from); len >= 1; len--)
        {
            string sub = hiragana.Substring(from, len);
            if (!RomaMap.TryGetValue(sub, out string[] r)) continue;
            char c = r[0][0];
            return ("aiueony".IndexOf(c) < 0) ? c : '\0';
        }
        return '\0';
    }
}
