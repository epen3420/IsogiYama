/// <summary>
/// 文単位でタイピングの正誤判定を行うクラス
/// </summary>
public class TypingJudder
{
    private char[] judeChars;
    private int judeCharsIndex = 0;

    public TypingJudder(string judgeString)
    {
        judeChars = judgeString.ToCharArray();
    }

    public TypingState JudgeChar(char typedChar)
    {
#if UNITY_EDITOR
        if (CheatModeWindow.IsCheat)
        {
            judeCharsIndex++;
            return judeCharsIndex >= judeChars.Length ? TypingState.Clear : TypingState.Hit;
        }
#endif
        if (judeChars[judeCharsIndex] == typedChar)
        {
            judeCharsIndex++;
            if (judeCharsIndex >= judeChars.Length)
            {
                return TypingState.Clear;
            }
            return TypingState.Hit;
        }
        return TypingState.Miss;
    }
}
