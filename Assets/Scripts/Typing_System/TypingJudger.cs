public class TypingJudger
{
    private char[] currentChars;
    private int currentCharIndex;

    public TypingJudger(string judgeString)
    {
        currentChars = judgeString.ToCharArray();
        currentCharIndex = 0;
    }

    public TypingState JudgeWord(char inputChar)
    {
        if (currentChars[currentCharIndex] == inputChar)
        {
            currentCharIndex++;
            if (currentCharIndex >= currentChars.Length)
            {
                return TypingState.Clear;
            }
            return TypingState.Hit;
        }
        return TypingState.Miss;
    }
}

public enum TypingState
{
    None = -1,
    Hit = 0,
    Miss = 1,
    Clear = 2
}
