// In CombatPrepData.cs
using System.Collections.Generic;
using UnityEngine;

public static class CombatPrepData
{
    public static List<FairyData> SelectedAllies = new List<FairyData>();
    public static List<FairyData> SelectedEnemies = new List<FairyData>();

    [Header("Backup")]
    public static List<FairyData> BackupSelectedAllies;
    public static float BackupGold;
    public static List<FairyData> BackupOwnedFairies; // Add this line
}
