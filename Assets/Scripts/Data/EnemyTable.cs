using System.Collections.Generic;
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
    //public int Move_Speed { get; set; }
    //public string Icon { get; set; }
    //public string Model { get; set; }

    public override string ToString()
    {
        //return $"{Unit_ID} / {Unit_Name} / {Unit_DEF_TYPE} / {Unit_DEF} / {Unit_HP} / {Stage} / {Move_Speed} / {Icon} / {Model}";
        return $"{Unit_ID} / {Unit_Name} /{Unit_DEF_TYPE} / {Unit_DEF} / {Unit_HP} / {Stage}";
    }

    //public Sprite SpriteIcon => Resources.Load<Sprite>($"{DefineNames.IconsFolder}/{Icon}");
    //public GameObject VisualModel => Resources.Load<GameObject>($"{DefineNames.EnemyModelFolder}/{Model}");
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
                Debug.LogError($"키 중복: {enemy.Unit_ID}");
            }
        }

        foreach(var enemy in dictionary)
        {
            Debug.Log(enemy.Value);

            var data = enemy.Value;
            //Debug.Log(data.Unit_Name);
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

    //TODO: Enemy 랜덤 로직 구현해야함
}
