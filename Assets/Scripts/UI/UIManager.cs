using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI StageText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI GoldText;

    public UIUnitInfo AllyInfoPanel;
    public UIUnitInfo EnemyInfoPanel;
    public Button UpgradeButton;

    private float surviveTime = 0f; //살아있는 시간
    private float second = 0f; //초 시간
    private float minute = 0f; //분 시간

    private void Update()
    {
        UpdateStage();
        UpdateTime();
        UpdateGold();
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

        if(prevMinute != minute)
        {
            Variables.Stage++;
        }

    }

    private void UpdateGold()
    {
        GoldText.text = $"Gold : {Variables.Gold}";
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
}
