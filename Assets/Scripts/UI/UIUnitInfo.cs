using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class UIUnitInfo : MonoBehaviour
{
    public UIManager uiManager;

    public TextMeshProUGUI Grade;
    public TextMeshProUGUI Unit_Name;
    public TextMeshProUGUI Damage;
    public TextMeshProUGUI AttackSpeed;

    public TextMeshProUGUI Deffense;
    public TextMeshProUGUI HP;
    public TextMeshProUGUI MoveSpeed;

    public void SetAllyInfo(AllyData data)
    {
        Grade.text = data.Unit_Grade.ToString();
        Unit_Name.text = data.Unit_Name;
        Damage.text = data.Unit_ATK.ToString();
        AttackSpeed.text = data.Unit_ATK_SPD.ToString();
    }

    public void SetEnemyInfo(EnemyUnit enemy)
    {
        Unit_Name.text = enemy.Data.Unit_Name;
        Deffense.text = enemy.Data.Unit_DEF.ToString();
        MoveSpeed.text = enemy.Data.Move_Speed.ToString();
        HP.text = $"HP : {enemy.Health}";
    }
}
