using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public Canvas speedCanvasButtonUI;
    public TextMeshProUGUI winLoseText;

    public bool combatEnded = false;

    public float goldRewardHolder = 0f;

    private void Awake()
    {
        goldRewardHolder = GoldManager.Instance.gold;   

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        speedCanvasButtonUI.enabled = true;
        combatCanvasUI.enabled = false;

        MatchingManager.Instance.playerFairy.Clear();
        MatchingManager.Instance.enemyFairy.Clear();

        foreach (var fairyData in CombatPrepData.SelectedAllies)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = allySpawnParent.position + new Vector3(randomOffset.x, 0, randomOffset.y);
            var allyGO = Instantiate(fairyData.FairyPrefab, spawnPos, Quaternion.identity, allySpawnParent);
            Debug.Log($"Spawning ally: {fairyData.FairyPrefab.name} at {spawnPos}");
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
        MatchingManager.Instance.MatchFairies();

        foreach (var fairy in MatchingManager.Instance.playerFairy.Concat(MatchingManager.Instance.enemyFairy))
        {
            var trackSystem = fairy.GetComponent<TrackSystem>();
            if (trackSystem != null)
                trackSystem.TryMatch();
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
            UpdateWinLoseText(alliesWin);
            StartCombatEndSequence();
            EnemyUnits.Instance.currentStageIndex++;
        }
        else
        {
            UpdateWinLoseText(alliesWin);
            Debug.Log("Enemies Win!");
            StartCombatEndSequence();
        }
        Time.timeScale = 1f; // Reset time scale to normal
        EnemyUnits.Instance.LoadStage(EnemyUnits.Instance.currentStageIndex);

    }
    private void StartCombatEndSequence()
    {
        combatCanvasUI.enabled = true;
        RewardsManager.Instance.goldReward = GoldManager.Instance.gold - goldRewardHolder;
        goldRewardHolder = 0f;
        RewardsManager.Instance.DisplayRewards();
        StageManager.Instance.ToggleStageTextVisibility();
        speedCanvasButtonUI.enabled = false;
    }
    public void ContinueToNextScene()
    {
        SceneManager.LoadScene("WaitingRoom");
        StageManager.Instance.ToggleStageTextVisibility();
    }

    public void UpdateWinLoseText(bool alliesWin)
    {
        if(alliesWin)
        {
            winLoseText.text = "Victory!";
            winLoseText.color = Color.green;
        }
        else
        {
            winLoseText.text = "Defeat!";
            winLoseText.color = Color.red;
        }
    }
}