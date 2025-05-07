using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : Singleton<GameFlowManager>
{
    private int stepIndex = 0;
    private float clearTime = 0.0f;
    private bool branchFlag = false;
    private GameStepBase nextStep;
    private GameStepBase currentGameStep;


    [SerializeField]
    private GameFlowDataBase gameFlowData;
    [SerializeField]
    private GameStepBase gameOverStep;


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

        var currentSceneName=SceneManager.GetActiveScene().name;
        var tryingTransionName=GetSceneNameByStepType(currentGameStep.StepType);
        if(currentSceneName!= tryingTransionName)
        {
            Debug.LogWarning($"The game flow order and scenes are different. So, we will transition to {tryingTransionName}.");

            GoToNextScene();
            return null;
        }
        stepIndex++;
        return currentGameStep.CsvFile;
    }

    public void SetClearTime(float time)
    {
        clearTime += time;
    }

    public void GoToNextScene()
    {
        if (currentGameStep is GameBranchStep gameBranchStep)
        {
            branchFlag = true;
            nextStep = gameBranchStep.GetNextStepByClearTime(clearTime);
            clearTime=0.0f;
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
        sceneLoader.LoadNextScene(GetSceneNameByStepType(nextStep.StepType));
        Debug.Log($"Next step: {nextStep.StepType}");
        
    }

    private string GetSceneNameByStepType(GameStepType stepType)
    {
        string value=null;
        switch (stepType)
        {
            case GameStepType.Story:
                value="StoryScene";
                break;
            case GameStepType.Typing:
               value= "TypingScene";
               break;
            default:
                Debug.LogError("Not set StepType in next GameStep.");
                break;
        }
        return value;
    }

    public void GameOver(){
        currentGameStep=gameOverStep;
        stepIndex=gameFlowData.gameSteps.Length;
        GoToNextScene();
    }
}
