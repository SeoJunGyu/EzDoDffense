using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private enum WarningType
    {
        None,
        EnemyOver100,
        BossTimer,
    }
    private WarningType currentWarning = WarningType.None;

    public static GameManager Instance { get; private set; }

    public GameTimer Timer { get; private set; } = new GameTimer();

    
    [SerializeField] private float timeScale = 5f;

    //FPS
    private float dt = 0f;
    [SerializeField] private Color color = Color.red;
    public TextMeshProUGUI FPSText;

    public UIManager uiManager;
    public EnemySpawner enemySpawner;
    public bool IsGameOver { get; private set; }

    public TextMeshProUGUI WarningTimer;
    private float enemyCountTimer = 30f;
    private float bossTimer = 60f;
    public TextMeshProUGUI WarningText;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = timeScale;
        Timer.TimerStart();

        WarningTimer.gameObject.SetActive(false);
        WarningText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Variables.Reset();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        //승리 확인
        CheckVictory();

        //패배 확인
        UpdateWarningUI();

        if (CheckEnemyOverCount() || CheckBossStageTimeout())
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
        Time.timeScale = uiManager.timeValue;
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
        if(Variables.Stage == 100 && Variables.BossSummoned && Variables.Boss == null && Variables.EnemyTotalCount <= 0)
        {
            VictoryGame();
        }
    }

    private void UpdateWarningUI()
    {
        bool enemyActive = Variables.EnemyTotalCount >= 100;
        bool bossActive = (Variables.Stage > 0 && Variables.Boss);

        var next = bossActive ? WarningType.BossTimer : (enemyActive ? WarningType.EnemyOver100 : WarningType.None);

        currentWarning = next;

        switch (currentWarning)
        {
            case WarningType.BossTimer:
                if (!Variables.IsBoss)
                {
                    Variables.IsBoss = true;
                    bossTimer = 60f;
                }

                if (bossTimer >= 55f)
                {
                    WarningText.gameObject.SetActive(true);
                    uiManager.ActiveBossSpawnText(true);
                    uiManager.BossNameText.text = Variables.Boss.UnitName;
                    WarningText.text = "1분안에 모험가를 잡으세요";
                }
                else
                {
                    uiManager.ActiveBossSpawnText(false);
                    WarningText.gameObject.SetActive(false);
                }

                WarningTimer.gameObject.SetActive(true);
                //uiManager.TimeText.gameObject.SetActive(false);
                uiManager.TimeTextGo.SetActive(false);

                bossTimer -= Time.deltaTime;
                WarningTimer.text = $"{Mathf.CeilToInt(Mathf.Max(0f, bossTimer))}";
                break;

            case WarningType.EnemyOver100:
                if (enemyCountTimer >= 25f)
                {
                    WarningText.gameObject.SetActive(true);
                    WarningText.text = "30초안에 100명 이하로 줄이세요";
                }
                else
                {
                    WarningText.gameObject.SetActive(false);
                }

                WarningTimer.gameObject.SetActive(true);
                //uiManager.TimeText.gameObject.SetActive(false);
                uiManager.TimeTextGo.SetActive(false);

                enemyCountTimer -= Time.deltaTime;
                WarningTimer.text = $"{Mathf.CeilToInt(Mathf.Max(0f, enemyCountTimer))}";
                break;

            case WarningType.None:
                Variables.IsBoss = false;
                bossTimer = 60f;
                enemyCountTimer = 30f;

                WarningTimer.gameObject.SetActive(false);
                WarningText.gameObject.SetActive(false);
                //uiManager.TimeText.gameObject.SetActive(true);
                uiManager.TimeTextGo.SetActive(true);
                break;
        }
    }

    private bool CheckEnemyOverCount()
    {
        return (currentWarning == WarningType.EnemyOver100) && enemyCountTimer <= 0f;
    }

    private bool CheckBossStageTimeout()
    {
        return (currentWarning == WarningType.BossTimer) && bossTimer <= 0f;
    }
}
