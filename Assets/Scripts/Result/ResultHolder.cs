public class TypingResultManager : Singleton<TypingResultManager>
{
    private TypingResult result = new TypingResult();

    public TypingResult GetResult() => result;

    public void SetResult(TypingResult r) => result = r;

    public void ClearResult() => result = new TypingResult();
}
