using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveData
{
    public float BgmVolume = 1f;
    public float SfxVolume = 1f;
    public int Tutorial = 0;
    public bool IsMute = false;
}

public static class SaveManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(SaveData data)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(SavePath, json);
    }

    public static SaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            return new SaveData(); // 기본값 반환
        }

        string json = File.ReadAllText(SavePath);
        return JsonConvert.DeserializeObject<SaveData>(json);
    }
}
