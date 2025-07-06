public class MapSegment
{
    public string Hiragana { get; set; }
    public string Romaji { get; set; }
    public int RomajiIndex { get; set; } = 0;
    public string CurrentInput { get; set; } //　現在までに入力されたローマ字 (部分一致判定用)

    public MapSegment(string hiragana, string romaji, int romajiIndex = 0)
    {
        Hiragana = hiragana;
        Romaji = romaji;
        RomajiIndex = romajiIndex;
        CurrentInput = ""; // 初期化
    }

    public override string ToString()
    {
        // Debug用にCurrentInputも表示するように変更
        return $"Hiragana: {Hiragana}, Roma: {Romaji} (Index: {RomajiIndex}')";
    }
}
