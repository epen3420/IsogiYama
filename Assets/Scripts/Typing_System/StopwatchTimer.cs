using UnityEngine;

public class StopwatchTimer : MonoBehaviour
{
    private bool isRunning = false;
    private float time = 0.0f;


    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public float GetTime()
    {
        return time;
    }

    public void ResetTimer()
    {
        time = 0.0f;
    }

    private void Update()
    {
        if (isRunning)
        {
            time += Time.deltaTime;
        }
    }
}
