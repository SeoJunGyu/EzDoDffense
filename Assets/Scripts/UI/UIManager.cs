using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI StageText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI GoldText;
    public TextMeshProUGUI EnemyCountText;

    public UIUnitInfo AllyInfoPanel;
    public UIUnitInfo EnemyInfoPanel;
    public Button UpgradeButton;

    private float surviveTime = 0f; //살아있는 시간
    private float second = 0f; //초 시간
    private float minute = 0f; //분 시간

    private float nextStageTime = 30f;

    //치트 패널
    public UICheat cheat;

    public GameObject GameOverUI;

    private void Awake()
    {
        cheat.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateStage();
        UpdateTime();
        UpdateGold();
        UpdateEnemyCount();
    }

    private void UpdateStage()
    {
        StageText.text = $"Stage {Variables.Stage}";
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
            Variables.Stage++;
            nextStageTime += 30f;
        }

    }

    private void UpdateGold()
    {
        GoldText.text = $"Gold : {Variables.Gold}";
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

        AllyInfoPanel.SetAllyInfo(data);
        AllyInfoPanel.gameObject.SetActive(true);

        EnemyInfoPanel.gameObject.SetActive(false);

        if(Variables.SelectedSlot.SocketInCount >= 3)
        {
            UpgradeButton.gameObject.SetActive(true);
        }
        else
        {
            UpgradeButton.gameObject.SetActive(false);
        }
    }

    public void ActiveInfoPanel(EnemyUnit enemy)
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

    public void SetActiveGameOverUi(bool IsGameOver)
    {

    }
}
