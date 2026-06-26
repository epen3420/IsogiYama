using UnityEngine;
using System.Diagnostics;
using UnityEngine.Profiling;

public class TypingBenchmark : MonoBehaviour
{
    private string testHiragana = "きょうはとくべつなきゅうじつなのできゅうこうれっしゃにのってとおくのまちへおでかけをしますしゃそうからみえるけしきはすばらしくちょっとしたたびきぶんをあじわうことができますとちゅうのえきでえきべんをかっておちゃをのみながらゆったりとしたじかんをすごしましたもくてきちにつくとにぎやかなしょうてんがいがありおきゃくさんでいっぱいでしたいろんなおみせをまわってあたらしいはっけんがたくさんありとてもまんぞくできるいちにちでした";
    private string testInput = "kyouhatokubetunakixyuuzitsunanodekyuukourextusyaninoltutetookunomatiheodekakewosimasusyasoukaramierukesikihasubarasikutiyoltutositatabikibunnwoaziwaukotogadekimasutotixyuunoekideekibennwokaxtuteotiyawonominagarayultsutaritositajikanwosugosimashitamokutekitinitukutonigiyakanasiyoutenngaigaariokilyakusandeixtsupaidesitaironnaomisewomawaltuteatarashiihaxtukenngatakusanaritotemomannzokudekiruitinichidesita";

    void Start()
    {
        RunBenchmark();
    }

    private void RunBenchmark()
    {
        // JITコンパイル等のウォームアップ
        for (int i = 0; i < 100; i++)
        {
            var judder = new TypingJudder(testHiragana);
            foreach (char c in testInput) judder.JudgeChar(c);
        }

        int iterations = 10000;
        Stopwatch sw = Stopwatch.StartNew();

        Profiler.BeginSample("TypingJudder_Benchmark");
        for (int i = 0; i < iterations; i++)
        {
            var judder = new TypingJudder(testHiragana);
            foreach (char c in testInput)
            {
                judder.JudgeChar(c);
            }
        }
        Profiler.EndSample();

        sw.Stop();

        double totalMs = sw.Elapsed.TotalMilliseconds;
        double perJudgeMs = totalMs / (iterations * testInput.Length);

        UnityEngine.Debug.Log($"[Benchmark] {iterations}回実行: {totalMs:F2} ms");
        UnityEngine.Debug.Log($"[Benchmark] 1文字判定あたり: {perJudgeMs:F6} ms");
        UnityEngine.Debug.Log("※Profilerを開いて「TypingJudder_Benchmark」のGC Allocを確認してください。");
    }
}
