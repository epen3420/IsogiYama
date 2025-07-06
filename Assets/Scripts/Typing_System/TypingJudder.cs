using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// 文単位でタイピングの正誤判定を行うクラス
/// </summary>
public class TypingJudder
{
    private int currentSegmentIndex = 0;

    public List<MapSegment> judgeList { get; private set; } = new List<MapSegment>();

    public int TotalRomajiLength => judgeList.Sum(segment => segment.Romaji?.Length ?? 0);

    public string FullRomajiText => string.Join("", judgeList.Select(segment => segment.Romaji));

    public event Action<string> OnRomajiTextChanged;

    public int GetCurrentInputLength()
    {
        int length = 0;
        for (int i = 0; i < currentSegmentIndex && i < judgeList.Count; i++)
        {
            length += judgeList[i].Romaji.Length;
        }
        if (currentSegmentIndex < judgeList.Count)
        {
            length += judgeList[currentSegmentIndex].CurrentInput.Length;
        }
        return length;
    }

    private static readonly Dictionary<string, string[]> JapaneseToRomaMap = new Dictionary<string, string[]>
    {
        {"あ", new string[] {"a"}},
        {"い", new string[] {"i", "yi"}},
        {"う", new string[] {"u", "wu", "whu"}},
        {"え", new string[] {"e"}},
        {"お", new string[] {"o"}},

        {"か", new string[] {"ka", "ca"}},
        {"き", new string[] {"ki"}},
        {"く", new string[] {"ku", "cu"}},
        {"け", new string[] {"ke"}},
        {"こ", new string[] {"ko", "co"}},

        {"さ", new string[] {"sa"}},
        {"し", new string[] {"sii", "shi", "ci"}},
        {"す", new string[] {"su"}},
        {"せ", new string[] {"se", "ce"}},
        {"そ", new string[] {"so"}},

        {"た", new string[] {"ta"}},
        {"ち", new string[] {"ti", "chi"}},
        {"つ", new string[] {"tuu", "tsu", "du"}},
        {"て", new string[] {"te"}},
        {"と", new string[] {"to"}},

        {"な", new string[] {"na"}},
        {"に", new string[] {"ni"}},
        {"ぬ", new string[] {"nu"}},
        {"ね", new string[] {"ne"}},
        {"の", new string[] {"no"}},

        {"は", new string[] {"ha"}},
        {"ひ", new string[] {"hi"}},
        {"ふ", new string[] {"hu", "fu"}},
        {"へ", new string[] {"he"}},
        {"ほ", new string[] {"ho"}},

        {"ま", new string[] {"ma"}},
        {"み", new string[] {"mi"}},
        {"む", new string[] {"mu"}},
        {"め", new string[] {"me"}},
        {"も", new string[] {"mo"}},

        {"や", new string[] {"ya"}},
        {"ゆ", new string[] {"yu"}},
        {"よ", new string[] {"yo"}},

        {"ら", new string[] {"ra"}},
        {"り", new string[] {"ri"}},
        {"る", new string[] {"ru"}},
        {"れ", new string[] {"re"}},
        {"ろ", new string[] {"ro"}},

        {"わ", new string[] {"wa"}},
        {"を", new string[] {"wo"}},
        {"ん", new string[] {"nn", "xn"}},

        {"が", new string[] {"ga"}},
        {"ぎ", new string[] {"gi"}},
        {"ぐ", new string[] {"gu"}},
        {"げ", new string[] {"ge"}},
        {"ご", new string[] {"go"}},

        {"ざ", new string[] {"za"}},
        {"じ", new string[] {"zi", "ji"}},
        {"ず", new string[] {"zu"}},
        {"ぜ", new string[] {"ze"}},
        {"ぞ", new string[] {"zo"}},

        {"だ", new string[] {"da"}},
        {"ぢ", new string[] {"di"}},
        {"づ", new string[] {"du"}},
        {"で", new string[] {"de"}},
        {"ど", new string[] {"do"}},

        {"ば", new string[] {"ba"}},
        {"び", new string[] {"bi"}},
        {"ぶ", new string[] {"bu"}},
        {"べ", new string[] {"be"}},
        {"ぼ", new string[] {"bo"}},

        {"ぱ", new string[] {"pa"}},
        {"ぴ", new string[] {"pi"}},
        {"ぷ", new string[] {"pu"}},
        {"ぺ", new string[] {"pe"}},
        {"ぽ", new string[] {"po"}},

        // 小さい仮名
        {"ぁ", new string[] {"xa", "la"}},
        {"ぃ", new string[] {"xi", "li"}},
        {"ぅ", new string[] {"xu", "lu"}},
        {"ぇ", new string[] {"xe", "le"}},
        {"ぉ", new string[] {"xo", "lo"}},
        {"ゃ", new string[] {"xya", "lya"}},
        {"ゅ", new string[] {"xyu", "lyu"}},
        {"ょ", new string[] {"xyo", "lyo"}},
        {"っ", new string[] {"ltu", "xtu", "xtsu", "ltsu"}},

        // 複合音
        {"きゃ", new string[] {"kya"}},
        {"きぃ", new string[] {"kyi"}},
        {"きゅ", new string[] {"kyu"}},
        {"きぇ", new string[] {"kye"}},
        {"きょ", new string[] {"kyo"}},

        {"くぁ", new string[] {"qa"}},
        {"くぃ", new string[] {"qi"}},
        {"くぅ", new string[] {"qwu"}},
        {"くぇ", new string[] {"qe"}},
        {"くぉ", new string[] {"qo"}},

        {"ぎゃ", new string[] {"gya"}},
        {"ぎぃ", new string[] {"gyi"}},
        {"ぎゅ", new string[] {"gyu"}},
        {"ぎぇ", new string[] {"gye"}},
        {"ぎょ", new string[] {"gyo"}},

        {"ぐぁ", new string[] {"gwa"}},
        {"ぐぃ", new string[] {"gwi"}},
        {"ぐぅ", new string[] {"gwu"}},
        {"ぐぇ", new string[] {"gwe"}},
        {"ぐぉ", new string[] {"gwo"}},

        {"しゃ", new string[] {"sya", "sha" }},
        {"しぃ", new string[] {"syi"}},
        {"しゅ", new string[] {"syu", "shu" }},
        {"しぇ", new string[] {"she", "sye" }},
        {"しょ", new string[] {"sho", "syo" }},

        {"すぁ", new string[] {"swa"}},
        {"すぃ", new string[] {"swi"}},
        {"すぅ", new string[] {"swu"}},
        {"すぇ", new string[] {"swe"}},
        {"すぉ", new string[] {"swo"}},

        {"じゃ", new string[] {"ja", "zya"}},
        {"じぃ", new string[] {"zyi"}},
        {"じゅ", new string[] {"ju", "zyu"}},
        {"じぇ", new string[] {"je", "zye"}},
        {"じょ", new string[] {"jo", "zyo"}},

        {"ちゃ", new string[] {"tya", "cha", "cya"}},
        {"ちぃ", new string[] {"tyi", "cyi"}},
        {"ちゅ", new string[] {"tyu", "chu", "cyu"}},
        {"ちぇ", new string[] {"tye", "che", "cye"}},
        {"ちょ", new string[] {"tyo", "cho", "cyo"}},

        {"てゃ", new string[] {"tha"}},
        {"てぃ", new string[] {"thi"}},
        {"てゅ", new string[] {"thu"}},
        {"てぇ", new string[] {"the"}},
        {"てょ", new string[] {"tho"}},

        {"とぁ", new string[] {"twa"}},
        {"とぃ", new string[] {"twi"}},
        {"とぅ", new string[] {"twu"}},
        {"とぇ", new string[] {"twe"}},
        {"とぉ", new string[] {"two"}},

        {"ぢゃ", new string[] {"dya"}},
        {"ぢぃ", new string[] {"dyi"}},
        {"ぢゅ", new string[] {"dyu"}},
        {"ぢぇ", new string[] {"dye"}},
        {"ぢょ", new string[] {"dyo"}},

        {"でゃ", new string[] {"dha"}},
        {"でぃ", new string[] {"dhi"}},
        {"でゅ", new string[] {"dhu"}},
        {"でぇ", new string[] {"dhe"}},
        {"でょ", new string[] {"dho"}},

        {"どぁ", new string[] {"dwa"}},
        {"どぃ", new string[] {"dwi"}},
        {"どぅ", new string[] {"dwu"}},
        {"どぇ", new string[] {"dwe"}},
        {"どぉ", new string[] {"dwo"}},

        {"にゃ", new string[] {"nya"}},
        {"にぃ", new string[] {"nyi"}},
        {"にゅ", new string[] {"nyu"}},
        {"にぇ", new string[] {"nye"}},
        {"にょ", new string[] {"nyo"}},

        {"ひゃ", new string[] {"hya"}},
        {"ひぃ", new string[] {"hyi"}},
        {"ひゅ", new string[] {"hyu"}},
        {"ひぇ", new string[] {"hye"}},
        {"ひょ", new string[] {"hyo"}},

        {"ふぁ", new string[] {"fa"}},
        {"ふぃ", new string[] {"fi"}},
        {"ふぅ", new string[] {"fwu"}},
        {"ふぇ", new string[] {"fe"}},
        {"ふぉ", new string[] {"fo"}},

        {"びゃ", new string[] {"bya"}},
        {"びぃ", new string[] {"byi"}},
        {"びゅ", new string[] {"byu"}},
        {"びぇ", new string[] {"bye"}},
        {"びょ", new string[] {"byo"}},

        {"ぴゃ", new string[] {"pya"}},
        {"ぴぃ", new string[] {"pyi"}},
        {"ぴゅ", new string[] {"pyu"}},
        {"ぴぇ", new string[] {"pye"}},
        {"ぴょ", new string[] {"pyo"}},

        {"みゃ", new string[] {"mya"}},
        {"みぃ", new string[] {"myi"}},
        {"みゅ", new string[] {"myu"}},
        {"みぇ", new string[] {"mye"}},
        {"みょ", new string[] {"myo"}},

        {"りゃ", new string[] {"rya"}},
        {"りぃ", new string[] {"ryi"}},
        {"りゅ", new string[] {"ryu"}},
        {"りぇ", new string[] {"rye"}},
        {"りょ", new string[] {"ryo"}},

        {"うぁ", new string[] {"wha"}},
        {"うぃ", new string[] {"whi"}},
        {"うぇ", new string[] {"whe"}},
        {"うぉ", new string[] {"who"}}
    };

    public static List<MapSegment> MappingHiraganaToRomaji(string hiraganaString)
    {
        var result = new List<MapSegment>();
        int currentIndex = 0;

        while (currentIndex < hiraganaString.Length)
        {
            bool foundMatch = false;

            // 「っ」の特殊処理
            if (hiraganaString[currentIndex] == 'っ')
            {
                // デフォは一番始めのローマ字を使用
                string romaToAssign = JapaneseToRomaMap["っ"][0];

                if (currentIndex + 1 < hiraganaString.Length)
                {
                    string nextHiragana = "";
                    // 次のひらがなを最長一致で検索
                    for (int len = Math.Min(3, hiraganaString.Length - (currentIndex + 1)); len >= 1; len--)
                    {
                        string sub = hiraganaString.Substring(currentIndex + 1, len);
                        if (JapaneseToRomaMap.ContainsKey(sub))
                        {
                            nextHiragana = sub;
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(nextHiragana) && JapaneseToRomaMap.ContainsKey(nextHiragana))
                    {
                        string nextRoma = JapaneseToRomaMap[nextHiragana][0];
                        // 次のローマ字が母音または 'n' 'y' 以外の子音で始まる場合
                        // ただし、最初の文字が母音の場合 (あ, い, う, え, お) は促音の対象外
                        if (nextRoma.Length > 0 && !"aiueon".Contains(char.ToLower(nextRoma[0])) && char.ToLower(nextRoma[0]) != 'y')
                        {
                            romaToAssign = nextRoma[0].ToString(); // その子音を取得
                        }
                    }
                }
                result.Add(new MapSegment("っ", romaToAssign));
                currentIndex++;
                foundMatch = true;
            }
            // 「ん」の特殊処理
            else if (hiraganaString[currentIndex] == 'ん')
            {
                result.Add(new MapSegment("ん", "nn"));
                currentIndex++;
                foundMatch = true;
            }
            // 通常の最長一致探索
            if (!foundMatch)
            {
                for (int length = Math.Min(3, hiraganaString.Length - currentIndex); length >= 1; length--)
                {
                    string subString = hiraganaString.Substring(currentIndex, length);

                    if (JapaneseToRomaMap.ContainsKey(subString))
                    {
                        result.Add(new MapSegment(subString, JapaneseToRomaMap[subString][0], 0));
                        currentIndex += length;
                        foundMatch = true;
                        break;
                    }
                }
            }

            if (!foundMatch)
            {
                // マッチするひらがなが見つからない場合
                result.Add(new MapSegment(hiraganaString[currentIndex].ToString(), "[UNMAPPED]"));
                currentIndex++;
            }
        }
        return result;
    }

    public TypingJudder(string hiragana)
    {
        judgeList = MappingHiraganaToRomaji(hiragana);
        currentSegmentIndex = 0;
    }

    /*
    public TypingState JudgeChar(char typedChar)
    {
#if UNITY_EDITOR
        if (CheatModeWindow.IsCheat)
        {
            currentSegmentIndex++;
            return currentSegmentIndex >= judeChars.Length ? TypingState.Clear : TypingState.Hit;
        }
#endif
        if (judeChars[currentSegmentIndex] == typedChar)
        {
            currentSegmentIndex++;
            if (currentSegmentIndex >= judeChars.Length)
            {
                return TypingState.Clear;
            }
            return TypingState.Hit;
        }
        return TypingState.Miss;
    }
    */

    public TypingState JudgeChar(char typedChar)
    {
        if (currentSegmentIndex >= judgeList.Count)
        {
            // 全てのひらがなセグメントの入力が完了済み
            return TypingState.Clear;
        }

        MapSegment currentSegment = judgeList[currentSegmentIndex];
        // 入力された文字を小文字に変換し、現在の入力文字列に追加
        char lowerTypedChar = char.ToLower(typedChar);
        string attemptedInput = currentSegment.CurrentInput + lowerTypedChar;

        // 現在設定されているローマ字表記での判定
        // currentSegment.Romaji の先頭が attemptedInput と一致するか
        if (currentSegment.Romaji.StartsWith(attemptedInput, StringComparison.OrdinalIgnoreCase))
        {
            currentSegment.CurrentInput = attemptedInput; // 入力を確定

            // 現在のセグメントのローマ字入力が完了したか
            if (currentSegment.CurrentInput.Length == currentSegment.Romaji.Length)
            {
                currentSegmentIndex++;
                return CheckIfCleared();
            }
            return TypingState.Hit;
        }

        // 「ん」の代替入力規則
        // 「ん」のセグメントで、かつ現在のローマ字表記では一致しなかった場合
        if (currentSegment.Hiragana == "ん")
        {
            // 「ん」の RomajiIndex を進めて、他の 'n' や 'xn' などの候補を試す
            string[] nRomajiOptions;
            if (JapaneseToRomaMap.TryGetValue("ん", out nRomajiOptions))
            {
                for (int i = currentSegment.RomajiIndex + 1; i < nRomajiOptions.Length; i++)
                {
                    if (nRomajiOptions[i].StartsWith(attemptedInput, StringComparison.OrdinalIgnoreCase))
                    {
                        currentSegment.Romaji = nRomajiOptions[i];
                        currentSegment.RomajiIndex = i;
                        currentSegment.CurrentInput = attemptedInput;

                        OnRomajiTextChanged?.Invoke(FullRomajiText); // ローマ字テキストが変更されたことを通知

                        if (currentSegment.CurrentInput.Length == currentSegment.Romaji.Length)
                        {
                            currentSegmentIndex++;
                            return CheckIfCleared();
                        }
                        return TypingState.Hit;
                    }
                }
            }

            // 「ん」の省略形 (次の文字が子音の場合) の可能性
            // 現在のセグメントが「ん」で、まだ入力が完了していないか、
            // または「n」の最初の1文字だけが入力されている場合
            if (currentSegment.CurrentInput.Length == 0 || (currentSegment.CurrentInput.Length == 1 && currentSegment.CurrentInput.ToLower() == "n"))
            {
                // 次のセグメントが存在するか、かつ、そのセグメントの最初のローマ字が入力された文字と一致するか
                if (currentSegmentIndex + 1 < judgeList.Count)
                {
                    MapSegment nextSegment = judgeList[currentSegmentIndex + 1];
                    string nextSegmentDefaultRoma = JapaneseToRomaMap.ContainsKey(nextSegment.Hiragana) ? JapaneseToRomaMap[nextSegment.Hiragana][0] : "";

                    // 次のセグメントのローマ字の最初の文字が、入力された文字と一致し、かつ母音、'n', 'y' 以外の場合
                    // (つまり、促音「っ」のように次の子音を先行入力することで「ん」を省略できるケース)
                    if (nextSegmentDefaultRoma.Length > 0 && char.ToLower(nextSegmentDefaultRoma[0]) == lowerTypedChar &&
                        !"aiueony".Contains(lowerTypedChar))
                    {
                        // currentSegment.Romaji = "n"; // 「ん」のローマ字を「n」に設定   
                        // 「ん」のセグメントをスキップし、次のセグメントへ移動
                        currentSegmentIndex++;
                        // 次のセグメントの CurrentInput も更新（入力された文字が、次のセグメントの最初の文字として扱われる）
                        nextSegment.CurrentInput = lowerTypedChar.ToString();

                        OnRomajiTextChanged?.Invoke(FullRomajiText); // ローマ字テキストが変更されたことを通知

                        if (nextSegment.CurrentInput.Length == nextSegment.Romaji.Length)
                        {
                            currentSegmentIndex++;
                            return CheckIfCleared();
                        }
                        return TypingState.Hit;
                    }
                }
            }
        }

        //その他の代替ローマ字表記での試行
        // 「ん」以外の通常のひらがなで、現在の Romaji では一致しなかった場合
        // JapaneseToRomaMap にそのひらがなが含まれていれば
        string[] allRomajiOptions;
        if (JapaneseToRomaMap.TryGetValue(currentSegment.Hiragana, out allRomajiOptions))
        {
            // 現在のRomajiIndex以降のオプションを試す
            for (int i = currentSegment.RomajiIndex + 1; i < allRomajiOptions.Length; i++)
            {
                string nextRomajiOption = allRomajiOptions[i];
                // 試行するオプションが attemptedInput で始まるか
                if (nextRomajiOption.StartsWith(attemptedInput, StringComparison.OrdinalIgnoreCase))
                {
                    currentSegment.Romaji = nextRomajiOption; // MapSegmentのRomajiプロパティを更新
                    currentSegment.RomajiIndex = i; // Indexも更新
                    currentSegment.CurrentInput = attemptedInput; // 入力を確定

                    OnRomajiTextChanged?.Invoke(FullRomajiText); // ローマ字テキストが変更されたことを通知

                    if (currentSegment.CurrentInput.Length == currentSegment.Romaji.Length)
                    {
                        currentSegmentIndex++;
                        return CheckIfCleared();
                    }
                    return TypingState.Hit;
                }
            }
        }

        // 複合ひらがなの動的分割試行
        // 現在のセグメントが複数文字のひらがなで、かつ、まだ入力が開始されていない場合
        if (currentSegment.Hiragana.Length > 1 && currentSegment.CurrentInput.Length == 0)
        {
            string firstChar = currentSegment.Hiragana.Substring(0, 1); // 最初のひらがな
            string remainingChars = currentSegment.Hiragana.Substring(1); // 残りのひらがな

            // 最初の1文字が単独のひらがなとしてJapaneseToRomaMapに存在し、
            // かつ残りの文字もJapaneseToRomaMapに存在する場合に分割を試みる
            if (JapaneseToRomaMap.ContainsKey(firstChar) && JapaneseToRomaMap.ContainsKey(remainingChars))
            {
                string[] firstCharRomajiOptions = JapaneseToRomaMap[firstChar];

                // 分割後の最初のひらがなの各ローマ字表記を試す
                foreach (string splitRomajiOption in firstCharRomajiOptions)
                {
                    // 分割後の最初のひらがなのローマ字が入力された文字で始まるか
                    if (splitRomajiOption.StartsWith(lowerTypedChar.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        // リストから現在のセグメントを削除
                        judgeList.RemoveAt(currentSegmentIndex);

                        // 新しいセグメントをリストに挿入
                        // 分割後の最初のひらがな
                        MapSegment newFirstSegment = new MapSegment(firstChar, splitRomajiOption, Array.IndexOf(firstCharRomajiOptions, splitRomajiOption));
                        // 分割後の残りのひらがな
                        MapSegment newRemainingSegment = new MapSegment(remainingChars, JapaneseToRomaMap[remainingChars][0]);

                        judgeList.Insert(currentSegmentIndex, newRemainingSegment);
                        judgeList.Insert(currentSegmentIndex, newFirstSegment);

                        // 挿入された最初のセグメントのCurrentInputを更新
                        newFirstSegment.CurrentInput = lowerTypedChar.ToString();

                        OnRomajiTextChanged?.Invoke(FullRomajiText); // ローマ字テキストが変更されたことを通知

                        if (newFirstSegment.CurrentInput.Length == newFirstSegment.Romaji.Length)
                        {
                            currentSegmentIndex++;
                            return CheckIfCleared();
                        }
                        return TypingState.Hit;
                    }
                }
            }
        }

        // どの試行でも一致しなかった場合
        return TypingState.Miss;
    }

    private TypingState CheckIfCleared()
    {
        if (currentSegmentIndex >= judgeList.Count)
        {
            return TypingState.Clear;
        }
        // 次のセグメントの CurrentInput をリセット
        // これがないと、次のセグメントに移動した際に前のセグメントの CurrentInput が残ってしまう可能性がある
        if (currentSegmentIndex < judgeList.Count)
        {
            judgeList[currentSegmentIndex].CurrentInput = "";
        }
        return TypingState.Hit;
    }
}
