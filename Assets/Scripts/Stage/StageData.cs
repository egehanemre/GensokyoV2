using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/Stage Data", order = 1)]
public class StageData : ScriptableObject
{
    [System.Serializable]
    public class EnemyEntry
    {
        public GameObject enemyPrefab;
        public int count;
    }

    public Stages stage;

    [Header("Enemy Groups by Difficulty")]
    public List<EnemyEntry> easyEnemies;
    public List<EnemyEntry> mediumEnemies;
    public List<EnemyEntry> hardEnemies;
}
public enum Stages
{
    Stage1,
    Stage2,
    Stage3,
    Stage4,
    Stage5
}
public enum StageDifficulty
{
    Easy,
    Medium,
    Hard
}