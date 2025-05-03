using System;
using System.Diagnostics;

public class StopwatchTimer
{
    private Stopwatch timer = new Stopwatch();
    public void StartTimer()
    {
         timer.Start();
    }

    public void StopTimer()
    {
         timer.Stop();
    }
    public double GetTimer()
    {
        TimeSpan currentTime = timer.Elapsed;

        return currentTime.TotalSeconds;
    }

    public void ResetTimer()
    {
        timer.Reset();
    }

    public void RestartTimer()
    {
        timer.Restart();
    }

}
