using UnityEngine;
using UnityEngine.UI;

public class UIUpgradePanel : UIPanel
{
    public UITypeEnforcePanel typePanel;
    public UIGradeEnforcePanel gradePanel;

    public Button EnforceButton;

    public override void Hide()
    {
        EnforceButton.gameObject.SetActive(true);
        base.Hide();
    }

    public void OnClickType()
    {
        base.Hide();
        typePanel.Show();
    }

    public void OnClickGrade()
    {
        base.Hide();
        gradePanel.Show();
    }
}
