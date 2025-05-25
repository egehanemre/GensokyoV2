using System.Collections.Generic;
using UnityEngine;

public class EnemyUnits : MonoBehaviour
{
    public static EnemyUnits Instance { get; private set; }

    [SerializeField] private List<GameObject> enemyFairies = new List<GameObject>();
    public List<GameObject> EnemyFairies => enemyFairies;

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

        var stage = stageDatabase.stages.Find(s => s.stage == stageIndex);

        foreach (var entry in stage.enemies)
        {
            for (int i = 0; i < entry.count; i++)
            {
                enemyFairies.Add(entry.enemyPrefab);
            }
        }
        currentStageIndex = stageIndex;
    }
}

