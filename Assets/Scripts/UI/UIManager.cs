using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI TimeText;

    private float surviveTime = 0f; //����ִ� �ð�
    private float second = 0f; //�� �ð�
    private float minute = 0f; //�� �ð�

    private void Update()
    {
        UpdateTime();
    }

    private void UpdateTime()
    {
        surviveTime = GameManager.Instance.Timer.Elapsed;
        second = surviveTime;
        int m = Mathf.FloorToInt(second / 60f);
        int s = Mathf.FloorToInt(second) % 60;               // �Ǵ�: int s = Mathf.FloorToInt(t - m * 60f);
        TimeText.text = $"{m:00} : {s:00}";

    }
}
