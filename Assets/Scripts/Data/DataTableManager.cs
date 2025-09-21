using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class DataTableManager
{
    private static readonly Dictionary<string, DataTable> tables = new Dictionary<string, DataTable>();

    static DataTableManager()
    {
        Init();
    }

    private static void Init()
    {
        var enemyTable = new EnemyTable();
        enemyTable.Load(DataTableIds.Enemy);
        tables.Add(DataTableIds.Enemy, enemyTable);

        var allRandomTable = new RandomPickupTable();
        allRandomTable.Load(DataTableIds.AllRandom);
        tables.Add(DataTableIds.AllRandom, allRandomTable);

        var normalRandomTable = new RandomPickupTable();
        normalRandomTable.Load(DataTableIds.NormalRandom);
        tables.Add(DataTableIds.NormalRandom, normalRandomTable);

        var piercingRandomTable = new RandomPickupTable();
        piercingRandomTable.Load(DataTableIds.PiercingRandom);
        tables.Add(DataTableIds.PiercingRandom, piercingRandomTable);

        var magicalRandomTable = new RandomPickupTable();
        magicalRandomTable.Load(DataTableIds.MagicalRandom);
        tables.Add(DataTableIds.MagicalRandom, magicalRandomTable);

        var allyTable = new AllyTable();
        allyTable.Load(DataTableIds.Ally);
        tables.Add(DataTableIds.Ally, allyTable);

        var singleSkillTable = new SingleSkillTable();
        singleSkillTable.Load(DataTableIds.SingleSkill);
        tables.Add(DataTableIds.SingleSkill, singleSkillTable);

        var multiSkillTable = new MultiSkillTable();
        multiSkillTable.Load(DataTableIds.MultiSkill);
        tables.Add(DataTableIds.MultiSkill, multiSkillTable);
    }

    public static EnemyTable EnemyTable
    {
        get
        {
            return Get<EnemyTable>(DataTableIds.Enemy);
        }
    }

    public static AllyTable AllyTable
    {
        get
        {
            return Get<AllyTable>(DataTableIds.Ally);
        }
    }

    public static RandomPickupTable AllRandomTable
    {
        get
        {
            return Get<RandomPickupTable>(DataTableIds.AllRandom);
        }
    }

    public static RandomPickupTable NormalRandomTable
    {
        get
        {
            return Get<RandomPickupTable>(DataTableIds.NormalRandom);
        }
    }

    public static RandomPickupTable PiercingRandomTable
    {
        get
        {
            return Get<RandomPickupTable>(DataTableIds.PiercingRandom);
        }
    }

    public static RandomPickupTable MagicalRandomTable
    {
        get
        {
            return Get<RandomPickupTable>(DataTableIds.MagicalRandom);
        }
    }

    public static SingleSkillTable SingleSkillTable
    {
        get
        {
            return Get<SingleSkillTable>(DataTableIds.SingleSkill);
        }
    }

    public static MultiSkillTable MultiSkillTable
    {
        get
        {
            return Get<MultiSkillTable>(DataTableIds.MultiSkill);
        }
    }

    public static T Get<T>(string id) where T : DataTable
    {
        if (!tables.ContainsKey(id))
        {
            Debug.LogError("테이블 없음");
            return null;
        }

        return tables[id] as T;
    }
}
