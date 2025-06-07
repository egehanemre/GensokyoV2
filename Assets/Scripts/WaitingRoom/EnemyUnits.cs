using System.Collections.Generic;
using UnityEngine;

public class EnemyUnits : MonoBehaviour
{
    public static EnemyUnits Instance { get; private set; }

    [SerializeField] private List<FairyData> enemyFairies = new List<FairyData>();
    public List<FairyData> EnemyFairies => enemyFairies;

    [Header("Stage System")]
    [SerializeField] private StageDatabase stageDatabase;
    public Stages currentStageIndex;

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
    private void Start()
    {
        if (StageManager.Instance != null)
            StageManager.Instance.UpdateStageUI();
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
            if (entry == null || entry.enemyPrefab == null)
            {
                Debug.LogWarning("Enemy entry or prefab is null.");
                continue;
            }
            for (int i = 0; i < entry.count; i++)
            {
                string uniqueId = System.Guid.NewGuid().ToString();
                enemyFairies.Add(new FairyData(uniqueId, entry.enemyPrefab));
            }
        }
        currentStageIndex = stageIndex;
    }
    public void ResetGame()
    {
        currentStageIndex = Stages.Stage1; // or your starting stage
        LoadStage(currentStageIndex);
    }
}