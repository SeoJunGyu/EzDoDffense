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
            if (Timer.IsRunning)
            {
                PauseGame();
            }
            else
            {
                ResumGame();
            }
        }

        //승리 확인
        CheckVictory();

        //패배 확인
        if (CheckEnemyOverCount())
        {
            uiManager.SetActiveGameOverUi();
        }
        else if (CheckBossStageTimeout())
        {
            uiManager.SetActiveGameOverUi();
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
        if (IsGameOver)
        {
            return;
        }
        IsGameOver = true;

        PauseGame();
        uiManager.SetActiveGameOverUi();
        enemySpawner.enabled = false;
    }

    public void VictoryGame()
    {
        if (IsGameOver)
        {
            return;
        }
        IsGameOver = true;

        PauseGame();
        uiManager.SetActiveVictoryUi();
        enemySpawner.enabled = false;
    }

    public void CheckVictory()
    {
        if(Variables.Stage == 100 && Variables.EnemyTotalCount <= 0)
        {
            uiManager.SetActiveVictoryUi();
        }
    }

    private float enemyOverCountTimer = 0f;
    public bool CheckEnemyOverCount()
    {
        if(Variables.EnemyTotalCount >= 100)
        {
            enemyOverCountTimer += Time.deltaTime;
            if(enemyOverCountTimer >= 30f)
            {
                return true;
            }
        }
        else
        {
            enemyOverCountTimer = 0f;
        }

        return false;
    }

    private float bossStageTimer = 0f;
    private bool CheckBossStageTimeout()
    {
        if(Variables.Stage > 0 && Variables.Boss)
        {
            if (!Variables.IsBoss)
            {
                Variables.IsBoss = true;
                bossStageTimer = 0f;
            }

            bossStageTimer += Time.deltaTime;

            if(bossStageTimer >= 60f)
            {
                return true;
            }
        }
        else
        {
            Variables.IsBoss = false;
            bossStageTimer = 0f;
        }

        return false;
    }
}
