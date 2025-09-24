using TMPro;
using UnityEngine;

public class UISkillInfo : UIPanel
{
    public TextMeshProUGUI skillText;

    public void SetSkillInfo(string SkillText)
    {
        skillText.text = SkillText;
    }
}
