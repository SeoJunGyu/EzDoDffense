using TMPro;
using UnityEngine;

public class UIGradeEnforcePanel : UIPanel
{
    public TextMeshProUGUI Normal;
    public TextMeshProUGUI Rare;
    public TextMeshProUGUI Unique;
    public TextMeshProUGUI Legend;
    public TextMeshProUGUI Epic;

    private void Update()
    {
        UpdateUpgradeGold();
    }

    private void UpdateUpgradeGold()
    {
        Normal.text = $"{PlacementManager.GradeUpgradeGold[1]}";
        Rare.text = $"{PlacementManager.GradeUpgradeGold[2]}";
        Unique.text = $"{PlacementManager.GradeUpgradeGold[3]}";
        if(PlacementManager.GradeUpgradeSave.ContainsKey(4) && PlacementManager.GradeUpgradeSave[4] >= 5)
        {
            Legend.text = $"{PlacementManager.GradeUpgradeGem[4]}";
        }
        else
        {
            Legend.text = $"{PlacementManager.GradeUpgradeGold[4]}";
        }

        if (PlacementManager.GradeUpgradeSave.ContainsKey(5) && PlacementManager.GradeUpgradeSave[5] >= 5)
        {
            Epic.text = $"{PlacementManager.GradeUpgradeGem[5]}";
        }
        else
        {
            Epic.text = $"{PlacementManager.GradeUpgradeGold[5]}";
        }
    }
}
