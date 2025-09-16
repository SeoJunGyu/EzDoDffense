using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI StageText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI GoldText;

    public GameObject InfoPanel;
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

    public void ActiveInfoPanel()
    {
        if(Variables.SelectedSlot.SocketInCount <= 0)
        {
            return;
        }

        InfoPanel.SetActive(true);

        if(Variables.SelectedSlot.SocketInCount >= 3)
        {
            UpgradeButton.gameObject.SetActive(true);
        }
    }

    public void ActiveInfoPanel(EnemyData data)
    {
        InfoPanel.SetActive(true);
        UpgradeButton.gameObject.SetActive(false);
    }

    public void UnActiveInfoPanel()
    {
        InfoPanel.SetActive(false);
    }
}
