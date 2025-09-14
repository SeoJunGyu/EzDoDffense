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
}

public static class Variables
{
    public static int Stage { get; set; } = 1;
    public static int EnemyTotalCount { get; set; } = 0;
    public static int AllyTotalCount { get; set; } = 20;
}
