using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGradeEnforcePanel : UIPanel
{
    public TextMeshProUGUI Normal;
    public TextMeshProUGUI Rare;
    public TextMeshProUGUI Unique;
    public TextMeshProUGUI Legend;
    public TextMeshProUGUI Epic;

    public Button EnforceButton;

    public Sprite Gem;
    public Image legendImage;
    public Image epicImage;

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
        Normal.text = $"{PlacementManager.Instance.GradeUpgradeGold[1]}";
        Rare.text = $"{PlacementManager.Instance.GradeUpgradeGold[2]}";
        Unique.text = $"{PlacementManager.Instance.GradeUpgradeGold[3]}";
        if(PlacementManager.Instance.GradeUpgradeSave.ContainsKey(4) && PlacementManager.Instance.GradeUpgradeSave[4] >= 5)
        {
            legendImage.sprite = Gem;
            Legend.text = $"{PlacementManager.Instance.GradeUpgradeGem[4]}";
        }
        else
        {
            Legend.text = $"{PlacementManager.Instance.GradeUpgradeGold[4]}";
        }

        if (PlacementManager.Instance.GradeUpgradeSave.ContainsKey(5) && PlacementManager.Instance.GradeUpgradeSave[5] >= 5)
        {
            epicImage.sprite = Gem;
            Epic.text = $"{PlacementManager.Instance.GradeUpgradeGem[5]}";
        }
        else
        {
            Epic.text = $"{PlacementManager.Instance.GradeUpgradeGold[5]}";
        }
    }
}
