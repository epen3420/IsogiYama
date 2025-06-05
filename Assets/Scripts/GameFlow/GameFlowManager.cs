using UnityEngine;
using SoundSystem;
using System.Linq;
using System.Collections.Generic;

public class GameFlowManager : Singleton<GameFlowManager>
{
    private const string TITLE_SCENE_NAME = "TitleScene";
    private const string TYPING_SCENE_NAME = "TypingScene";
    private const string STORY_SCENE_NAME = "StoryScene";
    private const string RESULT_SCENE_NAME = "ResultScene";
    private const string DEFAULT_BGM_NAME = "BGM";

    private float clearTime = 0.0f;

    private List<GameStep> gameSteps;
    private int currentStepIndex = 0;

    [Header("ゲームフローのデータベース")]
    [SerializeField]
    private GameFlowDataBase gameFlowData;


    private void Start()
    {
        if (gameFlowData == null)
        {
            Debug.LogError("GameFlowDataBase is not assigned.");
            return;
        }
        InitGameFlow();

        SoundPlayer.instance.PlayBgm(DEFAULT_BGM_NAME);
    }

    public TextAsset GetCurrentCSV()
    {
        var currentGameStep = gameSteps[currentStepIndex];

        if (currentGameStep is not GameStepNeedCSV csvStep)
        {
            throw new System.InvalidOperationException("Current GameStep does not support CSV access. Check gameFlowDB or GameStep type.");
        }

        if (csvStep.CsvFile == null)
        {
            throw new UnassignedReferenceException($"CSV file is not assigned in {csvStep}. Check the asset reference.");
        }

        Debug.Log($"Loaded CSV: {csvStep.CsvFile.name}");
        return csvStep.CsvFile;
    }


    public void AddClearTime(float time)
    {
        clearTime += time;
    }

    public void GoToNextScene(bool isGameOver = false)
    {
        GameStepType nextStepType;
        if (isGameOver)
        {
            nextStepType = GameStepType.Title;
            Debug.Log("Game Over. Returning to title after delay.");
            InitGameFlow();
        }
        else
        {
            nextStepType = GetNextStepType();
        }

        var nextScene = GetSceneNameByStepType(nextStepType);

        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogError("Scene name is null or empty.");
            return;
        }

        var sceneLoader = InstanceRegister.Get<SceneLoader>();
        sceneLoader.LoadNextScene(nextScene);
    }

    private void InitGameFlow()
    {
        gameSteps = gameFlowData.gameSteps.ToList();
        clearTime = 0.0f;
        currentStepIndex = 0;
        var result = ResultHolder.instance.GetResult();
        result.PrintSummary();
    }

    private GameStepType GetNextStepType()
    {
        var nextStepIndex = currentStepIndex + 1;
        if (gameSteps[currentStepIndex] is GameBranchStep branchStep)
        {
            var nextGameStep = branchStep.GetNextStepByClearTime(clearTime);
            gameSteps.Insert(nextStepIndex, nextGameStep);
            IncStepIndex();

            return nextGameStep.StepType;
        }

        if (nextStepIndex < gameSteps.Count)
        {
            IncStepIndex();
            return gameSteps[nextStepIndex].StepType;
        }

        // ここまで来たらもうすべてのGameStepを通ったことになる
        Debug.Log("All steps completed. Returning to title.");
        InitGameFlow();
        return GameStepType.Title;
    }

    private void IncStepIndex()
    {
        currentStepIndex++;
    }

    private string GetSceneNameByStepType(GameStepType stepType)
    {
        return stepType switch
        {
            GameStepType.Title => TITLE_SCENE_NAME,
            GameStepType.Story => STORY_SCENE_NAME,
            GameStepType.Typing => TYPING_SCENE_NAME,
            GameStepType.Result => RESULT_SCENE_NAME,
            _ => throw new System.ArgumentOutOfRangeException(nameof(stepType), $"Unhandled type: {stepType}")
        };
    }
}
