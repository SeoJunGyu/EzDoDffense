using TMPro;
using UnityEngine;

public class UISkillInfo : UIPanel
{
    public TextMeshProUGUI skillText;

    public override void Hide()
    {
        AudioManager.Instance.PlayClick();
        base.Hide();
    }

    public void SetSkillInfo(string SkillText)
    {
        skillText.text = SkillText;
    }
}
