using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI TimeText;

    private float surviveTime = 0f; //살아있는 시간
    private float second = 0f; //초 시간
    private float minute = 0f; //분 시간

    private void Update()
    {
        UpdateTime();
    }

    private void UpdateTime()
    {
        surviveTime = GameManager.Instance.Timer.Elapsed;
        second = surviveTime;
        int m = Mathf.FloorToInt(second / 60f);
        int s = Mathf.FloorToInt(second) % 60;               // 또는: int s = Mathf.FloorToInt(t - m * 60f);
        TimeText.text = $"{m:00} : {s:00}";

    }
}
