// Author(s): Paul Calande
// A looping timer that starts running after a set delay.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerDelayed
{
    // The timer for the delay.
    Timer timerDelay;
    // The timer that will loop after the delay.
    Timer timerLoop;
    // The callback that should happen every time the looping timer finishes.
    Timer.FinishedHandler loopFinishedCallback;

    public TimerDelayed(float secondsDelay, float secondsLoop,
        Timer.FinishedHandler loopFinishedCallback = null)
    {
        this.loopFinishedCallback = loopFinishedCallback;
        timerDelay = new Timer(secondsDelay, TimerDelay_Finished, false);
        timerLoop = new Timer(secondsLoop, loopFinishedCallback, true);
    }

    private void TimerDelay_Finished(float secondsOverflow)
    {
        // When the delay timer finishes, call the loop finished callback.
        loopFinishedCallback(secondsOverflow);
        // Enter the loop.
        timerLoop.Run(secondsOverflow);
    }

    public void Run(float secondsOverflow = 0.0f)
    {
        timerDelay.Run(secondsOverflow);
    }

    public void Clear()
    {
        timerDelay.Clear();
        timerLoop.Clear();
    }

    public void Stop()
    {
        timerDelay.Stop();
        timerLoop.Stop();
    }

    // Resets the timer so that it has to go through the delay again.
    public void Reset()
    {
        Clear();
        timerLoop.Stop();
    }

    public void Tick(float deltaTime)
    {
        timerDelay.Tick(deltaTime);
        timerLoop.Tick(deltaTime);
    }
}