using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum EnemyTypes
{
    LightArmor,
    HeavyArmor,
    MagiclaArmor,
    HeroArmor,
}

public class EnemyData
{
    public long Unit_ID { get; set; }
    public string Unit_Name { get; set; }
    public int Unit_DEF_TYPE { get; set; }
    public int Unit_DEF { get; set; }
    public int Unit_HP { get; set; }
    public int Stage { get; set; }
    public int Move_Speed { get; set; }
    public string Icon { get; set; }
    public string Model { get; set; }

    public override string ToString()
    {
        return $"{Unit_ID} / {Unit_Name} / {Unit_DEF_TYPE} / {Unit_DEF} / {Unit_HP} / {Stage} / {Move_Speed} / {Icon} / {Model}";
        //return $"{Unit_ID} / {Unit_Name} /{Unit_DEF_TYPE} / {Unit_DEF} / {Unit_HP} / {Stage}";
    }
    
    public Sprite SpriteUnitIcon => Resources.Load<Sprite>($"{DefineNames.EnemyIconsFolder}/{Icon}");
    public Sprite SpriteDEFTypeIcon
    {
        get
        {
            Sprite icon = null;
            switch (Unit_DEF_TYPE)
            {
                case 1:
                    icon = Resources.Load<Sprite>($"{DefineNames.UIIconsFolder}/Iron_Chestplate2");
                    break;
                case 2:
                    icon = Resources.Load<Sprite>($"{DefineNames.UIIconsFolder}/Iron_Chestplate7");
                    break;
                case 3:
                    icon = Resources.Load<Sprite>($"{DefineNames.UIIconsFolder}/MagicArmor_286");
                    break;
                case 4:
                    icon = Resources.Load<Sprite>($"{DefineNames.UIIconsFolder}/MagicArmor_286");
                    break;
            }

            return icon;
        }
    }
    public Sprite SpriteMoveSpeedIcon => Resources.Load<Sprite>($"{DefineNames.UIIconsFolder}/EnemyMoveSpeedIcon");
    [Ignore]
    public GameObject VisualModel => Resources.Load<GameObject>($"{DefineNames.EnemyModelFolder}/{Model}");

    public AttackTypes Advangage
    {
        get
        {
            AttackTypes type = AttackTypes.None;
            switch (Unit_DEF_TYPE)
            {
                case 1:
                    type = AttackTypes.Normal;
                    break;
                case 2:
                    type = AttackTypes.Magic;
                    break;
                case 3:
                    type = AttackTypes.Piercing;
                    break;
                case 4:
                    type = AttackTypes.None;
                    break;
            }

            return type;
        }
    }

    public AttackTypes Disadvangage
    {
        get
        {
            AttackTypes type = AttackTypes.None;
            switch (Unit_DEF_TYPE)
            {
                case 1:
                    type = AttackTypes.Piercing;
                    break;
                case 2:
                    type = AttackTypes.Normal;
                    break;
                case 3:
                    type = AttackTypes.Magic;
                    break;
                case 4:
                    type = AttackTypes.None;
                    break;
            }

            return type;
        }
    }
}

public class EnemyTable : DataTable
{
    private readonly Dictionary<long, EnemyData> dictionary = new Dictionary<long, EnemyData>();

    public override void Load(string filename)
    {
        dictionary.Clear();

        var path = string.Format(FormatPath, filename);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCSV<EnemyData>(textAsset.text);

        foreach(var enemy in list)
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

    public EnemyData Get(long id)
    {
        if (!dictionary.ContainsKey(id))
        {
            return null;
        }

        return dictionary[id];
    }

    public EnemyData GetStageEnemy(int stage)
    {
        return dictionary.Values.FirstOrDefault(e => e.Stage == stage);
    }
}
