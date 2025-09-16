using TMPro;
using UnityEngine;

public class UIUnitInfo : MonoBehaviour
{
    public UIManager uiManager;

    public TextMeshProUGUI Grade;
    public TextMeshProUGUI Unit_Name;
    public TextMeshProUGUI Damage;
    public TextMeshProUGUI AttackSpeed;

    private void OnEnable()
    {
        if (!Variables.SelectedSlot)
        {
            return;
        }

        var data = Variables.SelectedSlot.CurrentData;

        Grade.text = data.Unit_Grade.ToString();
        Unit_Name.text = data.Unit_Name;
        Damage.text = data.Unit_ATK.ToString();
        AttackSpeed.text = data.Unit_ATK_SPD.ToString();
    }
}
