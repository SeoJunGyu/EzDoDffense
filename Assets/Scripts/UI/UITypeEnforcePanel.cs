using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITypeEnforcePanel : UIPanel
{
    public TextMeshProUGUI Normal;
    public TextMeshProUGUI Piercing;
    public TextMeshProUGUI Magic;

    public Button EnforceButton;

    private void Update()
    {
        UpdateUpgradeGold();
    }

    public override void Hide()
    {
        EnforceButton.gameObject.SetActive(true);
        base.Hide();
    }

    private void UpdateUpgradeGold()
    {
        Normal.text = PlacementManager.Instance.TypeUpgradeGold[1] > 0 ? $"{PlacementManager.Instance.TypeUpgradeGold[1]}" : "MAX";
        Piercing.text = PlacementManager.Instance.TypeUpgradeGold[2] > 0 ? $"{PlacementManager.Instance.TypeUpgradeGold[2]}" : "MAX";
        Magic.text = PlacementManager.Instance.TypeUpgradeGold[3] > 0 ? $"{PlacementManager.Instance.TypeUpgradeGold[3]}" : "MAX";
    }
}
