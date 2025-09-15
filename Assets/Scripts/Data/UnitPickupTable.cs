using System.Collections.Generic;
using UnityEngine;

public class PickupData
{
    public long Unit_ID { get; set; }
    public float Random_P { get; set; }
}

public class RandomPickupTable : DataTable
{
    private readonly Dictionary<long, PickupData> dictionary = new Dictionary<long, PickupData>();

    public override void Load(string filename)
    {
        dictionary.Clear();

        var path = string.Format(FormatPath, filename);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCSV<PickupData>(textAsset.text);

        foreach (var enemy in list)
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
    }

    public PickupData Get(long id)
    {
        if (!dictionary.ContainsKey(id))
        {
            return null;
        }

        return dictionary[id];
    }

    public long GetRandomId()
    {
        float total = 0f;
        foreach(var kv in dictionary)
        {
            total += kv.Value.Random_P;
        }

        float rnd = Random.Range(0f, total); //난수 생성

        float cumulativeWeight = 0f; //확률을 누적해서 구간 정하기
        foreach(var kv in dictionary)
        {
            cumulativeWeight += kv.Value.Random_P;
            if(rnd < cumulativeWeight)
            {
                return kv.Key;
            }
        }

        return 0; //못찾을 경우
    }
}
