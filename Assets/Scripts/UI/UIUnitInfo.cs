using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUnitInfo : MonoBehaviour
{
    public UIManager uiManager;

    public TextMeshProUGUI Grade;
    public TextMeshProUGUI Unit_Name;
    public TextMeshProUGUI Damage;
    public TextMeshProUGUI AttackSpeed;
    public Image UnitImage;
    public Image DamageImage;
    public Image SpeedImage;
    public Image Skill1;
    public Image Skill2;

    public TextMeshProUGUI Deffense;
    public TextMeshProUGUI HP;
    public TextMeshProUGUI MoveSpeed;


    public void SetAllyInfo(AllyData data)
    {
        
        Unit_Name.text = data.Unit_Name;

        UnitImage.sprite = data.SpriteUnitIcon;
        DamageImage.sprite = data.SpriteDamageIcon;
        SpeedImage.sprite = data.SpriteATKSpeedIcon;

        if(data.Unit_Skill_1 != 0)
        {
            Skill1.gameObject.SetActive(true);
            Skill1.sprite = SkillManager.Instance.GetSingleData(data.Unit_Skill_1).SpriteSkillIcon;
        }
        else
        {
            Skill1.gameObject.SetActive(false);
        }

        if (data.Unit_Skill_2 != 0)
        {
            Skill2.gameObject.SetActive(true);
            Skill2.sprite = SkillManager.Instance.GetMultiData(data.Unit_Skill_2).SpriteSkillIcon;
        }
        else
        {
            Skill2.gameObject.SetActive(false);
        }
    }

    public void SetUnitCurrentInfo(AllyUnit unit)
    {
        string grade = null;
        switch (unit.Grade)
        {
            case 1:
                grade = "노멀";
                break;
            case 2:
                grade = "레어";
                break;
            case 3:
                grade = "유니크";
                break;
            case 4:
                grade = "레전드";
                break;
            case 5:
                grade = "에픽";
                break;
        }

        var gradeUpgrade = PlacementManager.GradeUpgradeSave.ContainsKey(unit.Grade) ? $"+{PlacementManager.GradeUpgradeSave[unit.Grade]}" : "";

        Grade.text = grade + gradeUpgrade;
        Damage.text = unit.Damage.ToString();
        AttackSpeed.text = unit.AtkSpeed.ToString();
    }

    public void SetEnemyInfo(EnemyUnit enemy)
    {
        Unit_Name.text = enemy.Data.Unit_Name;
        Deffense.text = enemy.Data.Unit_DEF.ToString();
        MoveSpeed.text = enemy.Data.Move_Speed.ToString();
        HP.text = enemy.Data.Unit_HP.ToString();

        UnitImage.sprite = enemy.Data.SpriteUnitIcon;
        DamageImage.sprite = enemy.Data.SpriteDEFTypeIcon;
        SpeedImage.sprite = enemy.Data.SpriteMoveSpeedIcon;
    }
}
