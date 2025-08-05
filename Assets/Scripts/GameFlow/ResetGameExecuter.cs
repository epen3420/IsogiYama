using UnityEngine;

public class ResetGameExecuter : MonoBehaviour
{
    public void Reset()
    {
        var flowManager = GameFlowManager.instance;
        flowManager.ResetGameFlow();
    }
}
