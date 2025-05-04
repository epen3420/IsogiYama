using UnityEngine;

public class GameFlowManager : Singleton<GameFlowManager>
{
    private GameStepBase currentGameStep;
    private int stepIndex = 0;
    private const float ERROR_CLEAR_TIME = 20.0f;

    public float ClearTime { private get; set; } = 0.0f;
    [SerializeField]
    private GameFlowDataBase gameFlowData;
    [SerializeField]
    private SceneLoader sceneLoader;


    private void Start()
    {
        if (gameFlowData == null)
        {
            Debug.LogError("GameFlowDataBase is not assigned in GameFlowManager.");
            return;
        }

        // 以下テスト用、実際は消す
        currentGameStep = gameFlowData.gameSteps[stepIndex++];
    }

    /// <summary>
    /// 現在のシーンで必要なCSVファイルを供給します。
    /// </summary>
    /// <returns></returns>
    public TextAsset GetCurrentCSV()
    {
        return currentGameStep.CsvFile;
    }

    public void GoToNextScene()
    {
        currentGameStep = gameFlowData.gameSteps[stepIndex++];

        if (currentGameStep is GameBranchStep branchStep)
        {
            currentGameStep = branchStep.GetNextStepByClearTime(ClearTime);
            if (currentGameStep != null)
            {
                sceneLoader.LoadNextScene("GameFlowDevScene");
                return;
            }
            else
            {
                Debug.LogError("No valid next step found for the current clear time.");
            }
        }

        switch (currentGameStep.StepType)
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
