using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI TimeText;

    private float surviveTime = 0f;
    private float second = 0f;
    private float minute = 0f;

    private void UpdateTime()
    {
        surviveTime += Time.deltaTime;
        second = surviveTime;
        if(second / 60 == 0)
        {
            minute++;
            second = 0f;
        }


    }
}
