using System;
using UnityEngine;

public static class DefineNames
{
    public static readonly string IconsFolder = "Icon";
    public static readonly string AllysModelFolder = "Prefabs/Allys";
    public static readonly string EnemyModelFolder = "Prefabs/Enemies";
}

public static class DataTableIds
{
    public static readonly string Enemy = "EnemyTable";
    public static readonly string Ally = "AllyTable";
    public static readonly string AllRandom = "AllRandomTable";
    public static readonly string NormalRandom = "NormalRandomTable";
    public static readonly string PiercingRandom = "PiercingRandomTable";
    public static readonly string MagicalRandom = "MagicalRandomTable";
}

public static class Variables
{
    public static int Stage { get; set; } = 1;
    public static int EnemyTotalCount { get; set; } = 0;
    public static int AllyTotalCount { get; set; } = 20;

    public static int SlotCount { get; set; } = 21; //현재 등록가능한 슬롯 수
    public static Clickable SelectedSlot { get; set; } //선택된 슬롯
}
