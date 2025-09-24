using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AttackTypes
{
    None,
    Normal,
    Piercing,
    Magic,
}

public struct StatMods
{
    public float AtkMul;
    public float AtkSpdMul;
}

public static class UpgradeDB
{
    private static Dictionary<int, float> GradeDamageMultiplier = new Dictionary<int, float>()
    {
        {1, 1.00f },
        {2, 1.10f },
        {3, 1.25f },
        {4, 1.40f },
        {5, 1.60f },
    };

    private static Dictionary<int, float> GradeAtkSpeedMultiplier = new Dictionary<int, float>()
    {
        {1, 0.16f },
        {2, 0.18f },
        {3, 0.20f },
        {4, 0.22f },
        {5, 0.25f },
    };

    public static int CalcGradeUpDamage(int baseAtk, int grade, int enhanceLevel)
    {
        if(!GradeDamageMultiplier.TryGetValue(grade, out float mul))
        {
            mul = 1f;
        }
        if(enhanceLevel <= 0)
        {
            return 0;
        }

        float result = baseAtk * mul * Mathf.Pow(1.08f, enhanceLevel);
        int calResult = Mathf.CeilToInt(result) - baseAtk;

        return calResult;
    }

    private const float Decay = 0.8f;
    public static float CalcGradeUpAtkSpeed(float baseAtkSpeed, int grade, int enhanceLevel)
    {
        if (!GradeAtkSpeedMultiplier.TryGetValue(grade, out float mul))
        {
            mul = 1f;
        }
        if(enhanceLevel <= 0)
        {
            return 0;
        }

        float result = baseAtkSpeed * (1f + mul * (1f - Mathf.Pow(Decay, enhanceLevel)));
        float calResult = (Mathf.Round(result * 100f) / 100f) - baseAtkSpeed;

        return calResult;
    }

    public static int CalcTypeUpDamage(int baseAtk, int enhanceLevel)
    {
        if (enhanceLevel <= 0)
        {
            return 0;
        }

        float result = baseAtk * Mathf.Pow(1.08f, enhanceLevel);
        int calResult = Mathf.CeilToInt(result) - baseAtk;

        return calResult;
    }

    private const float TypeDecay = 0.8f;
    public static float CalcTypeUpAtkSpeed(float baseAtkSpeed, int enhanceLevel)
    {
        if (enhanceLevel <= 0)
        {
            return 0;
        }

        float result = baseAtkSpeed * (1f + 0.2f * (1f - Mathf.Pow(TypeDecay, enhanceLevel)));
        float calResult = (Mathf.Round(result * 100f) / 100f) - baseAtkSpeed;

        return calResult;
    }
}

public class AllyData
{
    public long Unit_ID { get; set; }
    public string Unit_Name { get; set; }
    public int Unit_Type { get; set; }
    public int Unit_Grade { get; set; }
    public int Unit_ATK { get; set; }
    public float Unit_ATK_SPD { get; set; }
    public int Unit_ATK_RNG { get; set; }
    public int Unit_Move_Speed { get; set; }
    public long Unit_Skill_1 { get; set; }
    public long Unit_Skill_2 { get; set; }
    public string Icon { get; set; }
    public string Model { get; set; }

    public override string ToString()
    {
        return $"{Unit_ID} / {Unit_Name} / {Unit_Type} / {Unit_Grade} / {Unit_ATK} / {Unit_ATK_SPD} / {Unit_ATK_RNG} / {Unit_Move_Speed} / {Unit_Skill_1}/ {Unit_Skill_2} / {Icon} / {Model}";
        //return $"{Unit_ID} / {Unit_Name} /{Unit_DEF_TYPE} / {Unit_DEF} / {Unit_HP} / {Stage}";
    }

    public Sprite SpriteUnitIcon => Resources.Load<Sprite>($"{DefineNames.AllyIconsFolder}/{Icon}");
    public Sprite SpriteDamageIcon
    {
        get
        {
            Sprite spIcon = null;
            switch (Unit_Type)
            {
                case 1:
                    spIcon = Resources.Load<Sprite>($"{DefineNames.UIIconsFolder}/PictoIcon_Attack");
                    break;
                case 2:
                    spIcon = Resources.Load<Sprite>($"{DefineNames.UIIconsFolder}/PictoIcon_Bow");
                    break;
                case 3:
                    spIcon = Resources.Load<Sprite>($"{DefineNames.UIIconsFolder}/PictoIcon_Book");
                    break;
            }

            return spIcon;
        }
        
    }
    public Sprite SpriteATKSpeedIcon => Resources.Load<Sprite>($"{DefineNames.UIIconsFolder}/AllyATKSpeedIcon");

    [Ignore]
    public GameObject VisualModel => Resources.Load<GameObject>($"{DefineNames.AllysModelFolder}/{Model}");
}

public class AllyTable : DataTable
{
    private readonly Dictionary<long, AllyData> dictionary = new Dictionary<long, AllyData>();

    public override void Load(string filename)
    {
        dictionary.Clear();

        var path = string.Format(FormatPath, filename);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCSV<AllyData>(textAsset.text);

        foreach (var enemy in list)
        {
            if (!dictionary.ContainsKey(enemy.Unit_ID))
            {
                dictionary.Add(enemy.Unit_ID, enemy);
            }
            else
            {
                Debug.LogError($"Å° Áßº¹: {enemy.Unit_ID}");
            }
        }
    }

    public AllyData Get(long id)
    {
        if (!dictionary.ContainsKey(id))
        {
            return null;
        }

        return dictionary[id];
    }

    public AllyData GetAllRandom()
    {
        return dictionary[DataTableManager.AllRandomTable.GetRandomId()];
    }

    public AllyData GetNormalRandom()
    {
        return dictionary[DataTableManager.NormalRandomTable.GetRandomId()];
    }

    public AllyData GetPiercingRandom()
    {
        return dictionary[DataTableManager.PiercingRandomTable.GetRandomId()];
    }

    public AllyData GetMagicalRandom()
    {
        return dictionary[DataTableManager.MagicalRandomTable.GetRandomId()];
    }

    public AllyData GetUpgradeRandomId(int grade, int type)
    {
        var upgradeList = new List<KeyValuePair<long, AllyData>>();
        foreach(var kv in dictionary)
        {
            if(kv.Value.Unit_Grade == grade + 1 && kv.Value.Unit_Type == type)
            {
                upgradeList.Add(kv);
            }
        }

        return upgradeList[Random.Range(0, upgradeList.Count)].Value;
    }
}
