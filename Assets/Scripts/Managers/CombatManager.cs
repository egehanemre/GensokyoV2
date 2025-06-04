using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    #region Singleton & Inspector Fields

    public static CombatManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI countdownText;
    public Canvas combatCanvasUI;
    public Canvas speedCanvasButtonUI;
    public Canvas skillCanvas;
    public TextMeshProUGUI winLoseText;

    [Header("Spawning")]
    public Transform allySpawnParent;
    public Transform enemySpawnParent;
    public float spawnRadius = 5f;

    [Header("Managers")]
    public SpeedManager speedManager;

    #endregion

    #region State Fields

    private bool combatActive = false;
    public bool combatEnded = false;
    public float goldRewardHolder = 0f;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        goldRewardHolder = GoldManager.Instance.gold;
    }

    private void Start()
    {
        SetupUI();
        ClearFairyLists();
        SpawnAllies();
        SpawnEnemies();
        StartCoroutine(CombatCountdownCoroutine());
    }

    private void Update()
    {
        if (!combatEnded && combatActive)
        {
            StartCoroutine(CheckCombatEnd());
        }
    }

    #endregion

    #region Setup & Spawning

    private void SetupUI()
    {
        speedCanvasButtonUI.enabled = true;
        skillCanvas.enabled = true;
        combatCanvasUI.enabled = false;
    }

    private void ClearFairyLists()
    {
        MatchingManager.Instance.playerFairy.Clear();
        MatchingManager.Instance.enemyFairy.Clear();
    }

    private void SpawnAllies()
    {
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
    }

    private void SpawnEnemies()
    {
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

    #endregion

    #region Combat Flow

    private IEnumerator CombatCountdownCoroutine()
    {
        combatActive = false;
        if (countdownText != null)
            countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            if (countdownText != null)
                countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        if (countdownText != null)
        {
            countdownText.text = "Fight!";
            yield return new WaitForSeconds(0.7f);
            countdownText.gameObject.SetActive(false);
        }

        StartCombat();
    }

    private void StartCombat()
    {
        combatActive = true;
        StartFairyMatching();
        CombatSkillManager.Instance.SetManaGainActive(true);
    }

    private void StartFairyMatching()
    {
        MatchingManager.Instance.MatchFairies();
        foreach (var fairy in MatchingManager.Instance.playerFairy.Concat(MatchingManager.Instance.enemyFairy))
        {
            var trackSystem = fairy.GetComponent<TrackSystem>();
            if (trackSystem != null)
                trackSystem.TryMatch();
        }
    }

    private IEnumerator CheckCombatEnd()
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
        Time.timeScale = 1f;
        EnemyUnits.Instance.LoadStage(EnemyUnits.Instance.currentStageIndex);
    }

    private void StartCombatEndSequence()
    {
        StartCoroutine(SlowDownAndFreezeEffect());
        combatCanvasUI.enabled = true;
        RewardsManager.Instance.goldReward = GoldManager.Instance.gold - goldRewardHolder;
        goldRewardHolder = 0f;
        RewardsManager.Instance.DisplayRewards();
        StageManager.Instance.ToggleStageTextVisibility();
        speedCanvasButtonUI.enabled = false;
        skillCanvas.enabled = false;
    }

    private IEnumerator SlowDownAndFreezeEffect()
    {
        float targetTimeScale = 0.1f;
        float slowDuration = 0.5f;
        float freezeDuration = 4f;

        float startScale = Time.timeScale;
        float t = 0f;

        while (t < slowDuration)
        {
            t += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(startScale, targetTimeScale, t / slowDuration);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }
        Time.timeScale = targetTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(freezeDuration);

        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0.02f;
    }

    #endregion

    #region UI & Utility

    public void ContinueToNextScene()
    {
        SceneManager.LoadScene("WaitingRoom");
        Time.timeScale = 1f;
        StageManager.Instance.ToggleStageTextVisibility();
    }

    public void UpdateWinLoseText(bool alliesWin)
    {
        if (alliesWin)
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

    #endregion
}