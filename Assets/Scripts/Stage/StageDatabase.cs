using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageDatabase", menuName = "Game/Stage Database", order = 2)]
public class StageDatabase : ScriptableObject
{
    public List<StageData> stages;
    public float easyGoldRequirement;
    public float mediumGoldRequirement;
    public List<StageData.EnemyEntry> GetEnemiesForStage(Stages stage, StageDifficulty difficulty)
    {
        var stageData = stages.Find(s => s.stage == stage);
        if (stageData == null) return null;

        return difficulty switch
        {
            StageDifficulty.Easy => stageData.easyEnemies,
            StageDifficulty.Medium => stageData.mediumEnemies,
            StageDifficulty.Hard => stageData.hardEnemies,
            _ => stageData.easyEnemies
        };
    }

    public int GetRequiredFairyCount(Stages stage)
    {
        var stageData = stages.Find(s => s.stage == stage);
        return stageData != null ? stageData.requiredFairyCount : 4; 
    }

    public StageDifficulty GetDifficultyForGold()
    {
        if (GoldManager.Instance.gold < easyGoldRequirement)
            return StageDifficulty.Easy;
        if (GoldManager.Instance.gold < mediumGoldRequirement)
            return StageDifficulty.Medium;

        return StageDifficulty.Hard;
    }

}