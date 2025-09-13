using UnityEngine;

public class GameTimer
{
    public float Elapsed { get; private set; } //���� �ð�
    public bool IsRunning { get; private set; }

    public void Pause() => IsRunning = false;
    public void Resume() => IsRunning = true;
    public void Reset() => Elapsed = 0f;

    public void TimerStart()
    {
        Elapsed = 0f;
        IsRunning = true;
    }

    public void Tick(float delta)
    {
        if (!IsRunning)
        {
            return;
        }

        Elapsed += delta;
    }
}
