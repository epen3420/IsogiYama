using CSV4Unity;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

/// <summary>
/// タイピングシーン中の背景の管理をします
/// コンストラクタでCSVのデータとシーン内で使用するStopwatchTimerが必要です
/// Timerの時間とCSVのデータの時間を参照して背景を遷移させます
/// </summary>
public class TypingBGScheduler
{
    private VFXController vFXController;
    private StopwatchTimer timer;
    [Serializable]
    private struct TypingBGEvent
    {
        public string ImagePath;
        public float ExecuteTime;

        public TypingBGEvent(string imagePath, float executeTime)
        {
            ImagePath = imagePath;
            ExecuteTime = executeTime;
        }
    }
    private List<TypingBGEvent> bgEvents = new List<TypingBGEvent>();
    private int bgEventIndex = 0;
    private int displayTimeOfGameOverScreen;


    public TypingBGScheduler(CsvRow<TypingQuestType> csvFirstLineData,
                             StopwatchTimer timer,
                             Action<bool> endTypingScene,
                             string gameOverImageName,
                             float gameOverTime,
                             int displayTimeOfGameOverScreen
                            )
    {
        vFXController = InstanceRegister.Get<VFXController>();

        this.timer = timer;
        bgEvents.Add(new TypingBGEvent(gameOverImageName, gameOverTime));

        endTypingScene += End;

        this.displayTimeOfGameOverScreen = displayTimeOfGameOverScreen;

        StoreCSVData(csvFirstLineData);
        // 背景データを昇順にソート
        bgEvents.Sort((a, b) => a.ExecuteTime.CompareTo(b.ExecuteTime));

        StartBGCycle().Forget();
    }

    private async void End(bool isGameOver)
    {
        if (isGameOver)
        {


            await vFXController.ChangeBackgroundAsync(bgEvents[bgEvents.Count - 1].ImagePath, 0.0f);

            await UniTask.Delay(displayTimeOfGameOverScreen);
        }

        await vFXController.FadeInCanvasAsync();
    }

    public async UniTask FadeOut()
    {
        await vFXController.FadeOutCanvasAsync();
    }

    private async UniTask StartBGCycle()
    {
        while (bgEvents.Count >= bgEventIndex)
        {
            await ChangeBackground();
        }


        UnityEngine.Debug.Log("Complete to change background images.");
    }

    private void StoreCSVData(CsvRow<TypingQuestType> firstRow)
    {
        // タイピング中の背景データをリストに格納
        var initImagePath = firstRow[TypingQuestType.initBGImage].Get<string>();
        bgEvents.Add(new TypingBGEvent(initImagePath, 0.0f));
        for (int i = 1; TypingQuestType.japanese != (TypingQuestType)i; i += 2)
        {
            var currentTypingQuestType = (TypingQuestType)i;
            try
            {
                var imagePath = firstRow[currentTypingQuestType].Get<string>();
                var executeTime = firstRow[currentTypingQuestType + 1].Get<float>();
                bgEvents.Add(new TypingBGEvent(imagePath, executeTime));
            }
            catch
            {
                continue;
            }
        }
    }

    private async UniTask ChangeBackground(float changeDuration = 0.5f)
    {
        var currentBGEvent = bgEvents[bgEventIndex];
        await UniTask.WaitUntil(() => currentBGEvent.ExecuteTime <= timer.GetTime());

        vFXController.ChangeBackgroundAsync(currentBGEvent.ImagePath, changeDuration).Forget();
        bgEventIndex++;
        UnityEngine.Debug.Log($"Background changed to: {currentBGEvent.ImagePath}");
    }
}
