using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameTimer Timer { get; private set; } = new GameTimer();

    [SerializeField] private bool useUnScaledTime = false;
    [SerializeField] private float timeScale = 5f;

    //FPS
    private float dt = 0f;
    [SerializeField] private int size = 25;
    [SerializeField] private Color color = Color.red;
    public TextMeshProUGUI FPSText;

    public UIManager uiManager;
    public EnemySpawner enemySpawner;
    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        Instance = this;
        Time.timeScale = timeScale;
        Timer.TimerStart();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Variables.IsPaused = !Variables.IsPaused;
        }

        if (Variables.IsPaused)
        {
            Time.timeScale = 0f;
        }

        Timer.Tick(Time.deltaTime);

        //FPS
        dt += (Time.unscaledDeltaTime - dt) * 0.1f;

        float ms = dt * 1000f;
        float fps = 1.0f / dt;
        string text = string.Format("{0:0.} FPS ({1:0.0} ms)", fps, ms);

        FPSText.text = text;
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

    public void EndGame()
    {
        Variables.IsPaused = true;
        uiManager.SetActiveGameOverUi(IsGameOver);
        enemySpawner.enabled = false;
    }
}
