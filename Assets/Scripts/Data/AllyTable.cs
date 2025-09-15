using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEditor.ShaderGraph;
using UnityEngine;

public enum AttackTypes
{
    None,
    Normal,
    Piercing,
    Magic,
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

    public Sprite SpriteIcon => Resources.Load<Sprite>($"{DefineNames.IconsFolder}/{Icon}");
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

        foreach (var ally in dictionary)
        {
            Debug.Log(ally.Value);
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
}
