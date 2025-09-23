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
        Normal.text = $"필요 골드 : {PlacementManager.GradeUpgradeGold[1]}G";
        Rare.text = $"필요 골드 : {PlacementManager.GradeUpgradeGold[2]}G";
        Unique.text = $"필요 골드 : {PlacementManager.GradeUpgradeGold[3]}G";
        if(PlacementManager.GradeUpgradeSave.ContainsKey(4) && PlacementManager.GradeUpgradeSave[4] >= 5)
        {
            Legend.text = $"필요 보석 : {PlacementManager.GradeUpgradeGem[4]}개";
        }
        else
        {
            Legend.text = $"필요 골드 : {PlacementManager.GradeUpgradeGold[4]}G";
        }

        if (PlacementManager.GradeUpgradeSave.ContainsKey(5) && PlacementManager.GradeUpgradeSave[5] >= 5)
        {
            Epic.text = $"필요 보석 : {PlacementManager.GradeUpgradeGem[5]}개";
        }
        else
        {
            Epic.text = $"필요 골드 : {PlacementManager.GradeUpgradeGold[5]}G";
        }
    }
}
