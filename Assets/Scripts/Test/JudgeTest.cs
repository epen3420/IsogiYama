using UnityEngine;

namespace JudgeTest
{
    public class JudgeTest : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("--- JudgeTest Start ---");

            TestTyping("あいうえお", "aiueo");
            TestTyping("かきくけこ", "kakikukeko");

            TestTyping("しゃしゅしょ", "syasyusyo");
            TestTyping("ちゃちゅちょ", "tyatyutyo");

            TestTyping("がっこう", "gakkou");
            TestTyping("あった", "atta");
            TestTyping("まっく", "maxtuku");

            TestTyping("にほんご", "nihongo");
            TestTyping("しんぶん", "sinbunn");
            TestTyping("てんぷら", "tennpura");

            TestTyping("きゅうり", "kilyuwhuri");
            TestTyping("まりとっつぉ", "maritottso");

            TestTyping("ししし", "shisici");

            Debug.Log("--- JudgeTest End ---");
        }

        void TestTyping(string hiragana, string expectedRomajiInput)
        {
            Debug.Log($"\n--- Testing Hiragana: \"{hiragana}\" (Expected Romaji: \"{expectedRomajiInput}\") ---");

            TypingJudder judder = new TypingJudder(hiragana);
            judder.OnRomajiTextChanged += (newRomaji) =>
            {
                Debug.Log($"Romaji Text Changed: {newRomaji} (Current Input Length: {judder.GetCurrentInputLength()})");
            };

            Debug.Log($"Initial Full Romaji: {judder.FullRomajiText} (Initial Input Length: {judder.GetCurrentInputLength()})");

            foreach (char typedChar in expectedRomajiInput)
            {
                TypingState state = judder.JudgeChar(typedChar);
                Debug.Log($"Typed: '{typedChar}', State: {state}, Current Full Romaji: {judder.FullRomajiText}, Current Input Length: {judder.GetCurrentInputLength()}");

                if (state == TypingState.Miss)
                {
                    Debug.LogError($"Typing Missed for '{hiragana}' at char '{typedChar}'!");
                    break;
                }
            }

            if (judder.GetCurrentInputLength() == judder.TotalRomajiLength)
            {
                Debug.Log($"SUCCESS: \"{hiragana}\" cleared.");
            }
            else
            {
                Debug.LogError($"FAILURE: \"{hiragana}\" not fully cleared. Remaining romaji.");
            }
        }
    }
}
