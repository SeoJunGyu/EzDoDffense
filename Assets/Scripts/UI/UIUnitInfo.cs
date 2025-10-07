using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUnitInfo : UIPanel
{
    public UIManager uiManager;

    public TextMeshProUGUI Grade;
    public TextMeshProUGUI Unit_Name;
    public TextMeshProUGUI Damage;
    public TextMeshProUGUI AttackSpeed;
    public TextMeshProUGUI SalePrice;
    public Image UnitImage;
    public Image DamageImage;
    public Image SpeedImage;
    public Image Skill1;
    public Image Skill2;

    public TextMeshProUGUI Deffense;
    public TextMeshProUGUI HP;
    public TextMeshProUGUI MoveSpeed;

    public AllyData allyData;
    public UISkillInfo SkillInfo;

    public GameObject BattleInfo;

    private void OnEnable()
    {
        SkillInfo.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        SkillInfo.gameObject.SetActive(false);
        BattleInfo.gameObject.SetActive(false);
    }


    public void SetAllyInfo(AllyData data)
    {
        allyData = data;

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

        SalePrice.text = PlacementManager.Instance.UnitPrice[data.Unit_Grade - 1].ToString();
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

        var gradeUpgrade = PlacementManager.Instance.GradeUpgradeSave.ContainsKey(unit.Grade) ? $"+{PlacementManager.Instance.GradeUpgradeSave[unit.Grade]}" : "";
        var typeUpgrade = PlacementManager.Instance.TypeUpgradeSave.ContainsKey((int)unit.UnitType) ? $"{unit.UnitName} +{PlacementManager.Instance.TypeUpgradeSave[(int)unit.UnitType]}" : $"{unit.UnitName}";

        Grade.text = grade + gradeUpgrade;
        Unit_Name.text = typeUpgrade;
        Damage.text = unit.Damage.ToString("F2");
        AttackSpeed.text = unit.AtkSpeed.ToString("F2");
    }

    public void SetEnemyInfo(EnemyUnit enemy)
    {
        Unit_Name.text = enemy.Data.Unit_Name;
        Deffense.text = enemy.Data.Unit_DEF.ToString("F2");
        MoveSpeed.text = enemy.Data.Move_Speed.ToString("F2");
        HP.text = enemy.Data.Unit_HP.ToString();

        UnitImage.sprite = enemy.Data.SpriteUnitIcon;
        DamageImage.sprite = enemy.Data.SpriteDEFTypeIcon;
        SpeedImage.sprite = enemy.Data.SpriteMoveSpeedIcon;
    }

    public void ActiveSkillInfo(int index)
    {
        string skillInfo = null;
        if(index == 1 && allyData.Unit_Skill_1 != 0)
        {
            skillInfo = SkillManager.Instance.GetSingleData(allyData.Unit_Skill_1).Text;
        }
        else if(index == 2 && allyData.Unit_Skill_2 != 0)
        {
            skillInfo = SkillManager.Instance.GetMultiData(allyData.Unit_Skill_2).Text;
        }
        else
        {
            skillInfo = "스킬 없음";
        }

        SkillInfo.SetSkillInfo(skillInfo);
        SkillInfo.gameObject.SetActive(true);
        AudioManager.Instance.PlayClick();
    }

    public void ActiveBattleInfo()
    {
        AudioManager.Instance.PlayClick();
        BattleInfo.SetActive(true);
    }

    public void UnActiveBattleInfo()
    {
        AudioManager.Instance.PlayClick();
        BattleInfo.SetActive(false);
    }
}
