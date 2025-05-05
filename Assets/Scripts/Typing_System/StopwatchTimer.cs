using UnityEngine;

public class StopwatchTimer : MonoBehaviour
{
    private bool isRunning = false;
    private float time = 0.0f;


    public void StartTimer()
    {
        isRunning = true;
        Debug.Log($"Timer started at {time}s");
    }

    public void StopTimer()
    {
        isRunning = false;
        Debug.Log($"Timer stopped at {time}s");
    }

    public float GetTime()
    {
        return time;
    }

    public void ResetTimer()
    {
        time = 0.0f;
        Debug.Log("Timer reset to 0s");
    }

    private void Update()
    {
        if (isRunning)
        {
            time += Time.deltaTime;
        }
    }
}
