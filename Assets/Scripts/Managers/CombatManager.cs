using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public Transform allySpawnParent;
    public Transform enemySpawnParent;
    public float spawnRadius = 5f;
    public SpeedManager speedManager;
    public Canvas combatCanvasUI;

    public bool combatEnded = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        combatCanvasUI.enabled = false;

        MatchingManager.Instance.playerFairy.Clear();
        MatchingManager.Instance.enemyFairy.Clear();

        foreach (var fairyData in CombatPrepData.SelectedAllies)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = allySpawnParent.position + new Vector3(randomOffset.x, 0, randomOffset.y);
            var allyGO = Instantiate(fairyData.FairyPrefab, spawnPos, Quaternion.identity, allySpawnParent);
            var fairy = allyGO.GetComponent<Fairy>();
            if (fairy != null)
            {
                fairy.UniqueId = fairyData.UniqueId;
                MatchingManager.Instance.playerFairy.Add(fairy);
            }
        }

        foreach (var enemyData in CombatPrepData.SelectedEnemies)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = enemySpawnParent.position + new Vector3(randomOffset.x, 0, randomOffset.y);
            var enemyGO = Instantiate(enemyData.FairyPrefab, spawnPos, Quaternion.identity, enemySpawnParent);
            var fairy = enemyGO.GetComponent<Fairy>();
            if (fairy != null)
            {
                fairy.UniqueId = enemyData.UniqueId;
                MatchingManager.Instance.enemyFairy.Add(fairy);
            }
        }
    }

    private void Update()
    {
        if (!combatEnded)
        {
            StartCoroutine(CheckCombatEnd());
        }
    }

    private System.Collections.IEnumerator CheckCombatEnd()
    {
        while (!combatEnded)
        {
            MatchingManager.Instance.playerFairy.RemoveAll(f => f == null);
            MatchingManager.Instance.enemyFairy.RemoveAll(f => f == null);

            if (MatchingManager.Instance.playerFairy.Count == 0)
            {
                combatEnded = true;
                OnCombatEnd(false); 
            }
            else if (MatchingManager.Instance.enemyFairy.Count == 0)
            {
                combatEnded = true;
                OnCombatEnd(true); 
            }
            yield return null;
        }
    }
    private void OnCombatEnd(bool alliesWin)
    {
        if (alliesWin)
        {
            Debug.Log("Allies Win!");
            StartCombatEndSequence();
            EnemyUnits.Instance.currentStageIndex++;
        }
        else
        {
            Debug.Log("Enemies Win!");
            StartCombatEndSequence();
        }
        Time.timeScale = 1f; // Reset time scale to normal
        EnemyUnits.Instance.LoadStage(EnemyUnits.Instance.currentStageIndex);

    }
    private void StartCombatEndSequence()
    {
        combatCanvasUI.enabled = true;
    }
    public void ContinueToNextScene()
    {
        SceneManager.LoadScene("WaitingRoom");
    }
}