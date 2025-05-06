using UnityEngine;

public class GameFlowManager : Singleton<GameFlowManager>
{
    private int stepIndex = 0;
    private float clearTime = 0.0f;
    private bool branchFlag = false;
    private GameStepBase nextStep;
    private GameStepBase currentGameStep;


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

        if (branchFlag)
        {
            branchFlag = false;
            currentGameStep = ((GameBranchStep)gameFlowData.gameSteps[stepIndex - 1]).GetNextStepByClearTime(clearTime);
        }
        else
        {
            currentGameStep = gameFlowData.gameSteps[stepIndex];
        }

        // 通常のステップ処理
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
        if (currentGameStep is GameBranchStep gameBranchStep)
        {
            branchFlag = true;
            nextStep = gameBranchStep.GetNextStepByClearTime(clearTime);
        }
        else if (stepIndex < gameFlowData.gameSteps.Length)
        {
            nextStep = gameFlowData.gameSteps[stepIndex];
        }
        else
        {
            Debug.Log("All steps completed. No more steps to go.");
            return;
        }

        var sceneLoader = InstanceRegister.Get<SceneLoader>();
        Debug.Log($"Next step: {nextStep.StepType}");
        switch (nextStep.StepType)
        {
            case GameStepType.Story:
                sceneLoader.LoadNextScene("StoryScene");
                break;
            case GameStepType.Typing:
                sceneLoader.LoadNextScene("TypingScene");
                break;
            default:
                Debug.LogError("Not set StepType in next GameStep.");
                break;
        }
    }
}
