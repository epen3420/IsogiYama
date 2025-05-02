using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : SceneSingleton<GameFlowManager>
{
    private GameStep currentGameStep;
    private int stepIndex = 0;
    private const float ERROR_CLEAR_TIME = 20.0f;

    public float ClearTime { private get; set; } = 0.0f;
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
        return currentGameStep.csvFile;
    }

    // 注意: このメソッドは未完成です。
    // 特に、シーン遷移に関しては仮です。
    public void GoToNextScene()
    {
        currentGameStep = gameFlowData.gameSteps[stepIndex++];
        switch (currentGameStep.stepType)
        {
            case GameStepType.Story:
                SceneManager.LoadScene("StoryScene");
                break;
            case GameStepType.Typing:
                SceneManager.LoadScene("TypingScene");
                break;
            case GameStepType.Branch:
                currentGameStep = currentGameStep.GetNextStepByClearTime(ClearTime);
                SceneManager.LoadScene("StoryScene");
                break;
            default:
                Debug.LogError("Not set StepType in next GameStep.");
                break;
        }
    }
}
