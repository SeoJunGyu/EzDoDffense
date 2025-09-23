using UnityEngine;

public class UIUpgradePanel : UIPanel
{
    public UITypeEnforcePanel typePanel;
    public UIGradeEnforcePanel gradePanel;

    public void OnClickType()
    {
        Hide();
        typePanel.Show();
    }

    public void OnClickGrade()
    {
        Hide();
        gradePanel.Show();
    }
}
