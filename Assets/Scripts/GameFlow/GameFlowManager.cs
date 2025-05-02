using UnityEngine;

public class GameFlowManager : SceneSingleton<GameFlowManager>
{
    private int stepIndex = 0;
    private const float ERROR_TIME = 20.0f;

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

    public TextAsset GetCurrentCSVData()
    {
        var currentStep = gameFlowData.gameSteps[stepIndex++];
        if (currentStep.stepType == GameStepType.Branch && ClearTime > ERROR_TIME)
        {
            var branchStep = currentStep.GetNextStepByClearTime(ClearTime);
            return branchStep.csvFile;
        }
        return currentStep.csvFile;
    }
}
