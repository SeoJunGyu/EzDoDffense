using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI StageText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI GoldText;
    public TextMeshProUGUI GemText;
    public TextMeshProUGUI EnemyCountText;

    public UIUnitInfo AllyInfoPanel;
    public UIUnitInfo EnemyInfoPanel;
    public Button UpgradeButton;
    public Button DeleteButton;
    public Button EnforceButton;

    private float surviveTime = 0f; //살아있는 시간
    private float second = 0f; //초 시간
    private float minute = 0f; //분 시간

    private float nextStageTime = 30f;

    //치트 패널
    public UICheat cheat;

    public GameObject GameOverUI;
    public GameObject VictoryUI;

    public GameObject EnforcePanel;
    public GameObject TypeEnforcePanel;
    public GameObject GradeEnforcePanel;

    public GameObject BossSpawnText;
    public GameObject StageGo;
    public TextMeshProUGUI BossNameText;

    private void Awake()
    {
        cheat.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GameOverUI.SetActive(false);
        VictoryUI.SetActive(false);
        UpgradeButton.gameObject.SetActive(false);
        DeleteButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateStage();
        UpdateTime();
        UpdateGold();
        UpdateEnemyCount();

        if (!Variables.Boss)
        {
            BossSpawnText.SetActive(false);
            StageGo.SetActive(true);
        }
    }

    private void UpdateStage()
    {
        StageText.text = $"STAGE {Variables.Stage}";
    }

    private void UpdateTime()
    {
        surviveTime = GameManager.Instance.Timer.Elapsed;
        var prevMinute = minute;
        minute = Mathf.FloorToInt(surviveTime / 60f);
        second = Mathf.FloorToInt(surviveTime) % 60;
        TimeText.text = $"{minute:00} : {second:00}";

        if(surviveTime >= nextStageTime)
        {
            if (Variables.Stage >= 100)
            {
                Variables.Stage = 100;
                return;
            }
            else if (Variables.Boss)
            {
                nextStageTime = surviveTime + 30f;
                return;
            }
            Variables.Stage++;
            nextStageTime = surviveTime + 30f;
        }

    }

    private void UpdateGold()
    {
        GoldText.text = $"{Variables.Gold}";
        GemText.text = $"{Variables.Gem}";
    }

    private void UpdateEnemyCount()
    {
        EnemyCountText.text = $"{Variables.EnemyTotalCount} / 100";
    }

    public void ActiveInfoPanel(AllyData data)
    {
        if(Variables.SelectedSlot.SocketInCount <= 0)
        {
            return;
        }

        DeleteButton.gameObject.SetActive(true);

        AllyInfoPanel.SetAllyInfo(data);

        if(Variables.SelectedSlot.SocketInCount > 0)
        {
            AllyInfoPanel.SetUnitCurrentInfo(Variables.SelectedSlot.GetPrimaryUnit());
        }
        
        AllyInfoPanel.gameObject.SetActive(true);

        EnemyInfoPanel.gameObject.SetActive(false);

        if(Variables.SelectedSlot.SocketInCount >= 3 && Variables.SelectedSlot.CurrentData.Unit_Grade != 5)
        {
            UpgradeButton.gameObject.SetActive(true);
        }
        else
        {
            UpgradeButton.gameObject.SetActive(false);
        }

        if(Variables.SelectedSlot && Variables.SelectedSlot.SocketInCount != 0)
        {
            DeleteButton.gameObject.SetActive(true);
        }
        else
        {
            DeleteButton.gameObject.SetActive(false);
        }
    }

    public void ActiveInfoPanel(EnemyData enemy)
    {
        EnemyInfoPanel.SetEnemyInfo(enemy);
        EnemyInfoPanel.gameObject.SetActive(true);

        AllyInfoPanel.gameObject.SetActive(false);

        UpgradeButton.gameObject.SetActive(false);
    }

    public void UnActiveInfoPanel()
    {
        EnemyInfoPanel.gameObject.SetActive(false);
        AllyInfoPanel.gameObject.SetActive(false);
        DeleteButton.gameObject.SetActive(false);
        UpgradeButton.gameObject.SetActive(false);
    }

    public void ActiveCheat()
    {
        if (Variables.IsCheat)
        {
            cheat.gameObject.SetActive(false);
            Variables.IsCheat = false;
            return;
        }

        cheat.gameObject.SetActive(true);
        Variables.IsCheat = true;
    }

    public void SetActiveGameOverUi()
    {
        GameOverUI.SetActive(true);

        GameManager.Instance.PauseGame();
    }

    public void SetActiveVictoryUi()
    {
        VictoryUI.SetActive(true);

        GameManager.Instance.PauseGame();
    }

    public void ResetGame()
    {
        Variables.Reset();

        SceneManager.LoadScene(1);
    }

    public void BackGame()
    {
        SceneManager.LoadScene(0);
    }

    public void ActiveEnforcePanel()
    {
        AllyInfoPanel.gameObject.SetActive(false);
        EnforcePanel.SetActive(true);
        EnforceButton.gameObject.SetActive(false);
    }

    public void ActiveBossSpawnText(bool spawn)
    {
        BossSpawnText.SetActive(spawn);
        StageGo.SetActive(!spawn);
    }
}
