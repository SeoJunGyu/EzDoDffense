using System;
using UnityEngine;

public static class DefineNames
{
    public static readonly string AllyIconsFolder = "Icons/Allys";
    public static readonly string EnemyIconsFolder = "Icons/Enemies";
    public static readonly string SkillIconsFolder = "Icons/SkillAndUI";
    public static readonly string UIIconsFolder = "Icons/SkillAndUI";
    public static readonly string AllysModelFolder = "Prefabs/Allys";
    public static readonly string EnemyModelFolder = "Prefabs/Enemies";
    public static readonly string SkillEffectFolder = "Prefabs/SkillEffects";
    public static readonly string SoundFolder = "Sounds";
}

public static class DataTableIds
{
    public static readonly string Enemy = "EnemyTable";
    public static readonly string Ally = "AllyTable";
    public static readonly string AllRandom = "AllRandomTable";
    public static readonly string NormalRandom = "NormalRandomTable";
    public static readonly string PiercingRandom = "PiercingRandomTable";
    public static readonly string MagicalRandom = "MagicalRandomTable";
    public static readonly string SingleSkill = "SingleSkillTable";
    public static readonly string MultiSkill = "MultiSkillTable";
}

public static class Variables
{
    //치트
    public static bool IsCheat { get; set; } = false;

    public static bool IsBoss { get; set; } = false;
    public static EnemyUnit Boss { get; set; }

    public static int Stage { get; set; } = 1;
    public static int EnemyTotalCount { get; set; } = 0;
    public static int AllyTotalCount { get; set; } = 20;

    public static int SlotCount { get; set; } = 21; //현재 등록가능한 슬롯 수
    public static Clickable SelectedSlot { get; set; } //선택된 슬롯
    public static EnemyData SelectedEnemy { get; set; }

    public static int Gold { get; set; } = 150;
    public static int Gem { get; set; } = 0;

    public static void Reset()
    {
        IsCheat = false;
        IsBoss = false;
        Stage = 1;
        EnemyTotalCount = 0;
        SlotCount = 21;
        SelectedSlot = null;
        SelectedEnemy = null;
        Gold = 150;
        Gem = 0;
        Boss = null;
    }
}

public static class WinCondition
{
    public static event Action OnWin;
    public static void Trigger() => OnWin?.Invoke();
}

public static class LoseCondition
{
    public static event Action OnLose;
    public static void Trigger() => OnLose?.Invoke();
}
