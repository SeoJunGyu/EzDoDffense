using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using UnityEngine;

public class SingleSkillData
{
    public long Skill_ID { get; set; }
    public string Skill_Name { get; set; }
    public int Skill_Grade { get; set; }
    public int Skill_Type { get; set; }
    public int Skill_Effect { get; set; } //스킬 효과
    public int Skill_Effect_Value { get; set; }
    public int Skill_Target { get; set; }
    public int Skill_Area { get; set; }
    public int Skill_Duration { get; set; }
    public int Skill_Random { get; set; }
    public string Skill_Effect_Name { get; set; }
    public string Skill_Sound { get; set; }
    public string icon { get; set; }
    public string Text { get; set; }

    public override string ToString()
    {
        return $"{Skill_ID} / {Skill_Name} / {Skill_Grade} / {Skill_Type} / {Skill_Effect} / {Skill_Effect_Value} / {Skill_Target} / {Skill_Area} / {Skill_Duration}/ {Skill_Random} / {Skill_Effect_Name} / {Skill_Sound} / {icon} / {Text}";
    }

    public Sprite SpriteSkillIcon => Resources.Load<Sprite>($"{DefineNames.SkillIconsFolder}/{icon}");
    [Ignore]
    public ParticleSystem SkillParticle => Resources.Load<ParticleSystem>($"{DefineNames.SkillEffectFolder}/{Skill_Effect_Name}");
}

public class MultiSkillData
{
    public long Skill_ID { get; set; }
    public string Skill_Name { get; set; }
    public int Skill_Grade { get; set; }
    public int Skill_Type { get; set; }
    public int Skill_Random { get; set; }
    public int Skill_Target { get; set; }
    public int Skill_Area { get; set; }
    public int Skill_Effect_1 { get; set; } //스킬 효과
    public int Skill_Effect_Value_1 { get; set; }
    public int Skill_Duration_1 { get; set; }
    public int Skill_Effect_2 { get; set; } //스킬 효과
    public int Skill_Effect_Value_2 { get; set; }
    public float Skill_Duration_2 { get; set; }
    public string Skill_Effect_Name { get; set; }
    public string Skill_Sound { get; set; }
    public string icon { get; set; }
    public string Text { get; set; }

    public override string ToString()
    {
        return $"{Skill_ID} / {Skill_Name} / {Skill_Grade} / {Skill_Type} / {Skill_Target} / {Skill_Area} / {Skill_Effect_1} / {Skill_Effect_Value_1} / {Skill_Duration_1} / {Skill_Effect_2} / {Skill_Effect_Value_2} / {Skill_Duration_2} / {Skill_Random} / {Skill_Effect_Name} / {Skill_Sound} / {icon} / {Text}";
    }

    public Sprite SpriteSkillIcon => Resources.Load<Sprite>($"{DefineNames.SkillIconsFolder}/{icon}");
    [Ignore]
    public ParticleSystem SkillParticle => Resources.Load<ParticleSystem>($"{DefineNames.SkillEffectFolder}/{Skill_Effect_Name}");
}

public class SingleSkillTable : DataTable
{
    private readonly Dictionary<long, SingleSkillData> dictionary = new Dictionary<long, SingleSkillData>();

    public override void Load(string filename)
    {
        dictionary.Clear();

        var path = string.Format(FormatPath, filename);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCSV<SingleSkillData>(textAsset.text);

        foreach(var skill in list)
        {
            if (!dictionary.ContainsKey(skill.Skill_ID))
            {
                dictionary.Add(skill.Skill_ID, skill);
            }
            else
            {
                Debug.LogError($"키 중복: {skill.Skill_ID}");
            }
        }
    }

    public SingleSkillData Get(long id)
    {
        if (!dictionary.ContainsKey(id))
        {
            return null;
        }

        return dictionary[id];
    }
}

public class MultiSkillTable : DataTable
{
    private readonly Dictionary<long, MultiSkillData> dictionary = new Dictionary<long, MultiSkillData>();

    public override void Load(string filename)
    {
        dictionary.Clear();

        var path = string.Format(FormatPath, filename);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCSV<MultiSkillData>(textAsset.text);

        foreach (var skill in list)
        {
            if (!dictionary.ContainsKey(skill.Skill_ID))
            {
                dictionary.Add(skill.Skill_ID, skill);
            }
            else
            {
                Debug.LogError($"키 중복: {skill.Skill_ID}");
            }
        }
    }

    public MultiSkillData Get(long id)
    {
        if (!dictionary.ContainsKey(id))
        {
            return null;
        }

        return dictionary[id];
    }
}
