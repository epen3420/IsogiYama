using System.Collections.Generic;
using System;

/// <summary>
/// ひらがな列全体に対応する Trie 木をプリビルドし、静的キャッシュとして保持する。
/// アプリ起動時に GetOrBuild() を1回呼ぶだけでよい。
/// 同じひらがな列が再出題されても new TrieNode は一切発生しない。
/// </summary>
public static class TrieBuilder
{
    // ひらがな列全体をキーとする Trie ルートのキャッシュ
    private static readonly Dictionary<string, TrieNode> RootCache = new();

    // -------------------------------------------------------------------------
    // ひらがな → ローマ字 対応マップ
    // -------------------------------------------------------------------------
    public static readonly Dictionary<string, string[]> RomaMap = new()
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
        {"きゃ", new[] {"kya"}},  {"きぃ", new[] {"kyi"}},  {"きゅ", new[] {"kyu"}},
        {"きぇ", new[] {"kye"}},  {"きょ", new[] {"kyo"}},
        {"くぁ", new[] {"qa"}},   {"くぃ", new[] {"qi"}},   {"くぅ", new[] {"qwu"}},
        {"くぇ", new[] {"qe"}},   {"くぉ", new[] {"qo"}},
        {"ぎゃ", new[] {"gya"}},  {"ぎぃ", new[] {"gyi"}},  {"ぎゅ", new[] {"gyu"}},
        {"ぎぇ", new[] {"gye"}},  {"ぎょ", new[] {"gyo"}},
        {"ぐぁ", new[] {"gwa"}},  {"ぐぃ", new[] {"gwi"}},  {"ぐぅ", new[] {"gwu"}},
        {"ぐぇ", new[] {"gwe"}},  {"ぐぉ", new[] {"gwo"}},
        {"しゃ", new[] {"sya", "sha"}}, {"しぃ", new[] {"syi"}},
        {"しゅ", new[] {"syu", "shu"}}, {"しぇ", new[] {"she", "sye"}},
        {"しょ", new[] {"sho", "syo"}},
        {"すぁ", new[] {"swa"}},  {"すぃ", new[] {"swi"}},  {"すぅ", new[] {"swu"}},
        {"すぇ", new[] {"swe"}},  {"すぉ", new[] {"swo"}},
        {"じゃ", new[] {"ja", "zya"}},  {"じぃ", new[] {"zyi"}},
        {"じゅ", new[] {"ju", "zyu"}},  {"じぇ", new[] {"je", "zye"}},
        {"じょ", new[] {"jo", "zyo"}},
        {"ちゃ", new[] {"tya", "cha", "cya"}}, {"ちぃ", new[] {"tyi", "cyi"}},
        {"ちゅ", new[] {"tyu", "chu", "cyu"}}, {"ちぇ", new[] {"tye", "che", "cye"}},
        {"ちょ", new[] {"tyo", "cho", "cyo"}},
        {"つぁ", new[] {"tsa"}},  {"つぃ", new[] {"tsi"}},
        {"つぇ", new[] {"tse"}},  {"つぉ", new[] {"tso"}},
        {"てゃ", new[] {"tha"}},  {"てぃ", new[] {"thi"}},  {"てゅ", new[] {"thu"}},
        {"てぇ", new[] {"the"}},  {"てょ", new[] {"tho"}},
        {"とぁ", new[] {"twa"}},  {"とぃ", new[] {"twi"}},  {"とぅ", new[] {"twu"}},
        {"とぇ", new[] {"twe"}},  {"とぉ", new[] {"two"}},
        {"ぢゃ", new[] {"dya"}},  {"ぢぃ", new[] {"dyi"}},  {"ぢゅ", new[] {"dyu"}},
        {"ぢぇ", new[] {"dye"}},  {"ぢょ", new[] {"dyo"}},
        {"でゃ", new[] {"dha"}},  {"でぃ", new[] {"dhi"}},  {"でゅ", new[] {"dhu"}},
        {"でぇ", new[] {"dhe"}},  {"でょ", new[] {"dho"}},
        {"どぁ", new[] {"dwa"}},  {"どぃ", new[] {"dwi"}},  {"どぅ", new[] {"dwu"}},
        {"どぇ", new[] {"dwe"}},  {"どぉ", new[] {"dwo"}},
        {"にゃ", new[] {"nya"}},  {"にぃ", new[] {"nyi"}},  {"にゅ", new[] {"nyu"}},
        {"にぇ", new[] {"nye"}},  {"にょ", new[] {"nyo"}},
        {"ひゃ", new[] {"hya"}},  {"ひぃ", new[] {"hyi"}},  {"ひゅ", new[] {"hyu"}},
        {"ひぇ", new[] {"hye"}},  {"ひょ", new[] {"hyo"}},
        {"ふぁ", new[] {"fa"}},   {"ふぃ", new[] {"fi"}},   {"ふぅ", new[] {"fwu"}},
        {"ふぇ", new[] {"fe"}},   {"ふぉ", new[] {"fo"}},
        {"びゃ", new[] {"bya"}},  {"びぃ", new[] {"byi"}},  {"びゅ", new[] {"byu"}},
        {"びぇ", new[] {"bye"}},  {"びょ", new[] {"byo"}},
        {"ぴゃ", new[] {"pya"}},  {"ぴぃ", new[] {"pyi"}},  {"ぴゅ", new[] {"pyu"}},
        {"ぴぇ", new[] {"pye"}},  {"ぴょ", new[] {"pyo"}},
        {"みゃ", new[] {"mya"}},  {"みぃ", new[] {"myi"}},  {"みゅ", new[] {"myu"}},
        {"みぇ", new[] {"mye"}},  {"みょ", new[] {"myo"}},
        {"りゃ", new[] {"rya"}},  {"りぃ", new[] {"ryi"}},  {"りゅ", new[] {"ryu"}},
        {"りぇ", new[] {"rye"}},  {"りょ", new[] {"ryo"}},
        {"うぁ", new[] {"wha"}},  {"うぃ", new[] {"whi"}},
        {"うぇ", new[] {"whe"}},  {"うぉ", new[] {"who"}},
    };

    // -------------------------------------------------------------------------
    // 公開 API
    // -------------------------------------------------------------------------

    /// <summary>
    /// ひらがな列に対応する Trie のルートノードを返す。
    /// 初回は構築してキャッシュ、2回目以降はキャッシュを返す。
    /// </summary>
    public static TrieNode GetOrBuild(string hiragana)
    {
        if (RootCache.TryGetValue(hiragana, out TrieNode cached))
            return cached;

        TrieNode root = Build(hiragana);
        RootCache[hiragana] = root;
        return root;
    }

    // -------------------------------------------------------------------------
    // セグメント型
    // -------------------------------------------------------------------------

    /// <summary>
    /// ひらがな列を分解した最小単位。
    /// 1セグメント = 1つのひらがな（または複合音）。
    /// </summary>
    private readonly struct Segment
    {
        /// <summary>このセグメントのひらがな文字列（文字数カウント用）</summary>
        public readonly string Hiragana;

        /// <summary>通常ローマ字候補（複合音を含む）</summary>
        public readonly string[] Romajis;

        /// <summary>複合音を分割入力した場合の候補。null なら分割なし。</summary>
        public readonly string[] SplitRomajis;

        /// <summary>「っ」の子音重ね候補文字。'\0' なら不要。</summary>
        public readonly char ConsonantCarry;

        /// <summary>「ん」の省略入力候補文字。'\0' なら不要。</summary>
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

    // -------------------------------------------------------------------------
    // 内部：Trie 構築
    // -------------------------------------------------------------------------

    private static TrieNode Build(string hiragana)
    {
        var root = new TrieNode();
        var segments = ParseSegments(hiragana);
        InsertSegments(root, segments, 0, 0, 0);
        return root;
    }

    // -------------------------------------------------------------------------
    // セグメント分解
    // -------------------------------------------------------------------------

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
                    // 複合音: 分割入力候補を事前計算
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

    /// <summary>複合音（例: きゃ）を分割入力した場合の全候補を返す。</summary>
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

    /// <summary>「っ」の次の先頭子音を返す。子音重ね対象外なら '\0'。</summary>
    private static char GetConsonantCarry(string hiragana, int from)
    {
        if (from >= hiragana.Length) return '\0';
        for (int len = Math.Min(3, hiragana.Length - from); len >= 1; len--)
        {
            string sub = hiragana.Substring(from, len);
            if (!RomaMap.TryGetValue(sub, out string[] r)) continue;
            char c = r[0][0];
            return (!"aiueon".Contains(c) && c != 'y') ? c : '\0';
        }
        return '\0';
    }

    /// <summary>「ん」の次の先頭子音を返す。省略対象外なら '\0'。</summary>
    private static char GetNShortcut(string hiragana, int from)
    {
        if (from >= hiragana.Length) return '\0';
        for (int len = Math.Min(3, hiragana.Length - from); len >= 1; len--)
        {
            string sub = hiragana.Substring(from, len);
            if (!RomaMap.TryGetValue(sub, out string[] r)) continue;
            char c = r[0][0];
            return (!"aiueony".Contains(c)) ? c : '\0';
        }
        return '\0';
    }

    // -------------------------------------------------------------------------
    // Trie への挿入
    // -------------------------------------------------------------------------

    /// <summary>
    /// segments[segIdx] 以降を node 以下に挿入する。
    /// hiraCount / romaCount はここまでの累積カウント。
    /// </summary>
    private static void InsertSegments(
        TrieNode node, List<Segment> segments,
        int segIdx, int hiraCount, int romaCount)
    {
        if (segIdx >= segments.Count) return;

        Segment seg = segments[segIdx];
        int nextHira = hiraCount + seg.Hiragana.Length;
        int nextSeg = segIdx + 1;

        // --- 通常ローマ字候補 ---
        foreach (string roma in seg.Romajis)
            InsertString(node, roma, segments, nextSeg, nextHira, romaCount);

        // --- 複合音の分割入力候補 ---
        if (seg.SplitRomajis != null)
            foreach (string roma in seg.SplitRomajis)
                InsertString(node, roma, segments, nextSeg, nextHira, romaCount);

        // --- 「っ」子音重ね ---
        // 正しい実装: 子音重ねは「子音1文字で っ 完了」ではなく
        // 「子音重ね + 次セグメントのローマ字を連結したパス」として挿入する。
        // 例: って → tte = [子音 t] + [次セグ te] を連結して "tte" のパスを作る。
        // こうすることで t→t→e の正しいパスができ、
        // 1打鍵目の t で終端にならない。
        if (seg.ConsonantCarry != '\0' && nextSeg < segments.Count)
        {
            Segment nextSegment = segments[nextSeg];
            int nextNextHira = nextHira + nextSegment.Hiragana.Length;
            int nextNextSeg = nextSeg + 1;

            // 次セグメントの各ローマ字候補のうち、ConsonantCarry で始まるものを連結
            foreach (string nextRoma in nextSegment.Romajis)
            {
                if (nextRoma.Length == 0 || nextRoma[0] != seg.ConsonantCarry) continue;
                // 子音重ね文字 + 次セグメントのローマ字 = 連結文字列として1パスに
                string combined = seg.ConsonantCarry + nextRoma;
                InsertString(node, combined, segments, nextNextSeg, nextNextHira, romaCount);
            }

            // 次セグメントが複合音で分割入力する場合も考慮
            if (nextSegment.SplitRomajis != null)
            {
                foreach (string splitRoma in nextSegment.SplitRomajis)
                {
                    if (splitRoma.Length == 0 || splitRoma[0] != seg.ConsonantCarry) continue;
                    string combined = seg.ConsonantCarry + splitRoma;
                    InsertString(node, combined, segments, nextNextSeg, nextNextHira, romaCount);
                }
            }
        }

        // --- 「ん」省略入力 ---
        // "n" 1文字で ん が完了し、次セグメントの入力へ続く
        if (seg.NShortcut != '\0')
            InsertString(node, "n", segments, nextSeg, nextHira, romaCount);
    }

    /// <summary>
    /// roma を node 以下に文字列として挿入し、
    /// 末尾に到達したら終端ノードにカウントを記録して
    /// 次のセグメント列を続けて挿入する。
    /// </summary>
    private static void InsertString(
        TrieNode node, string roma,
        List<Segment> segments, int nextSegIdx,
        int hiraCount, int romaCountBefore)
    {
        TrieNode cur = node;
        foreach (char c in roma)
        {
            if (!cur.Children.TryGetValue(c, out TrieNode child))
            {
                child = new TrieNode();
                cur.Children[c] = child;
            }
            cur = child;
        }

        // 終端ノードにカウントを記録
        // 同じノードに複数のパスが合流する場合は先に設定された値を維持する
        // （どちらの表記で打っても同じひらがな数になるため問題なし）
        cur.IsTerminal = true;
        cur.HiraganaCount = hiraCount;
        cur.RomajiCount = romaCountBefore + roma.Length;

        // 次のセグメント列を続けて挿入
        InsertSegments(cur, segments, nextSegIdx, hiraCount, romaCountBefore + roma.Length);
    }
}
