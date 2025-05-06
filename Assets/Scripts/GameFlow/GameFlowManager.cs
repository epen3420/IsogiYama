using UnityEngine;

public class GameFlowManager : Singleton<GameFlowManager>
{
    private int stepIndex = 0;
    private float clearTime = 0.0f;
    private bool branchFlag = false;
    

    [SerializeField]
    private GameFlowDataBase gameFlowData;


    private void Start()
    {
        if (gameFlowData == null)
        {
            Debug.LogError("GameFlowDataBase is not assigned in GameFlowManager.");
            return;
        }
    }

    /// <summary>
    /// 現在のシーンで必要なCSVファイルを供給します。
    /// </summary>
    /// <returns></returns>
    public TextAsset GetCurrentCSV()
    {
        var currentGameStep = gameFlowData.gameSteps[stepIndex];

        if (branchFlag)
        {
            branchFlag = false;
            var nextStep = ((GameBranchStep)gameFlowData.gameSteps[stepIndex - 1]).GetNextStepByClearTime(clearTime);

            Debug.Log($"Branching to {nextStep.name}'s CSV");
            return nextStep.CsvFile;
        }

        branchFlag = currentGameStep.HasBranch;
        
        Debug.Log($"Load {currentGameStep.name}'s CSV");
        stepIndex++;
        return currentGameStep.CsvFile;
    }

    public void SetClearTime(float time)
    {
        clearTime = time;
    }

    public void GoToNextScene()
    {
        if (stepIndex >= gameFlowData.gameSteps.Length)
        {
            Debug.Log("All steps completed. No more steps to go.");
            return;
        }

        var nextStep = gameFlowData.gameSteps[stepIndex];

        var sceneLoader = InstanceRegister.Get<SceneLoader>();


        Debug.Log($"Next step: {nextStep.StepType}");
        switch (nextStep.StepType)
        {
            case GameStepType.Story:
                sceneLoader.LoadNextScene("StoryScene");
                break;
            case GameStepType.Typing:
                sceneLoader.LoadNextScene("GameFlowDevScene");
                break;
            default:
                Debug.LogError("Not set StepType in next GameStep.");
                break;
        }
    }
}
