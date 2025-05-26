using System.Collections.Generic;
using UnityEngine;

public class EnemyUnits : MonoBehaviour
{
    public static EnemyUnits Instance { get; private set; }

    [SerializeField] private List<FairyData> enemyFairies = new List<FairyData>();
    public List<FairyData> EnemyFairies => enemyFairies;

    [Header("Stage System")]
    [SerializeField] private StageDatabase stageDatabase;
    public Stages currentStageIndex = Stages.Stage1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(this.gameObject);

        LoadStage(currentStageIndex);
    }

    public void LoadStage(Stages stageIndex)
    {
        enemyFairies.Clear();

        if (stageDatabase == null)
        {
            Debug.LogError("StageDatabase is not assigned in EnemyUnits.");
            return;
        }

        StageDifficulty difficulty = stageDatabase.GetDifficultyForGold();
        var enemyEntries = stageDatabase.GetEnemiesForStage(stageIndex, difficulty);

        if (enemyEntries == null)
        {
            Debug.LogError($"No enemy entries found for stage: {stageIndex} with difficulty: {difficulty}");
            return;
        }

        foreach (var entry in enemyEntries)
        {
            for (int i = 0; i < entry.count; i++)
            {
                string uniqueId = System.Guid.NewGuid().ToString();
                enemyFairies.Add(new FairyData(uniqueId, entry.enemyPrefab));
            }
        }
        currentStageIndex = stageIndex;
        StageManager.Instance.UpdateStageUI();
    }
}