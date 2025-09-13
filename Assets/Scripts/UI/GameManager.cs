using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameTimer Timer { get; private set; } = new GameTimer();

    [SerializeField] private bool useUnScaledTime = false;

    private void Awake()
    {
        Instance = this;
        Timer.TimerStart();
    }

    private void Update()
    {
        Timer.Tick(Time.deltaTime);
    }

    public void PauseGame()
    {
        Timer.Pause();
        Time.timeScale = 0f;
    }

    public void ResumGame()
    {
        Time.timeScale = 1f;
        Timer.Resume();
    }

    public void ResetTimer()
    {
        Timer.Reset();
    }
}
