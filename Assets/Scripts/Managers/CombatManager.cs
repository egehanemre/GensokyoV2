using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public Animator transitionAnimator;
    public static int consecutiveLosses = 0;
    #region Singleton & Inspector Fields

    [Header("Arenas")]
    [SerializeField] private GameObject[] arenas; // Assign in Inspector, order should match Stages enum

    [Header("Look Targets")]
    public Transform allyLookTarget;
    public Transform enemyLookTarget;
    public static CombatManager Instance { get; private set; }

    [Header("Spawn Areas")]
    public SpawnArea allyMeleeSpawnArea;
    public SpawnArea allyRangedSpawnArea;
    public SpawnArea enemyMeleeSpawnArea;
    public SpawnArea enemyRangedSpawnArea;

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
        EnableArenaForCurrentStage();
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
            Fairy prefabFairy = fairyData.FairyPrefab.GetComponent<Fairy>();
            bool isMelee = prefabFairy != null && prefabFairy.fairyType == FairyType.Melee;

            Vector3 spawnPos = isMelee
                ? (allyMeleeSpawnArea != null ? allyMeleeSpawnArea.GetRandomPointInArea() : allySpawnParent.position)
                : (allyRangedSpawnArea != null ? allyRangedSpawnArea.GetRandomPointInArea() : allySpawnParent.position);

            // Example: Allies look right (90°)
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.right, Vector3.up);

            var allyGO = Instantiate(fairyData.FairyPrefab, spawnPos, lookRotation, allySpawnParent);
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
            Fairy prefabFairy = enemyData.FairyPrefab.GetComponent<Fairy>();
            bool isMelee = prefabFairy != null && prefabFairy.fairyType == FairyType.Melee;

            Vector3 spawnPos = isMelee
                ? (enemyMeleeSpawnArea != null ? enemyMeleeSpawnArea.GetRandomPointInArea() : enemySpawnParent.position)
                : (enemyRangedSpawnArea != null ? enemyRangedSpawnArea.GetRandomPointInArea() : enemySpawnParent.position);

            // Example: Enemies look left (270°)
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.left, Vector3.up);

            var enemyGO = Instantiate(enemyData.FairyPrefab, spawnPos, lookRotation, enemySpawnParent);
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

    public static bool ShouldRestoreBackup = false;

    private void OnCombatEnd(bool alliesWin)
    {
        if (alliesWin)
        {
            consecutiveLosses = 0; // Reset on win
            Debug.Log("Allies Win!");
            UpdateWinLoseText(alliesWin);
            StartCombatEndSequence();

            if (EnemyUnits.Instance.currentStageIndex == Stages.Stage5) //hardcoded last stage index.
            {
                ShowWinScreen();
            }
            else
            {
                EnemyUnits.Instance.currentStageIndex++;
                EnemyUnits.Instance.LoadStage(EnemyUnits.Instance.currentStageIndex);
            }
        }
        else
        {
            consecutiveLosses++;
            UpdateWinLoseText(alliesWin);
            Debug.Log("Enemies Win!");
            StartCombatEndSequence();

            // Restore gold and units for retry
            GoldManager.Instance.SetGold(CombatPrepData.BackupGold);
            PlayerUnits.Instance.OwnedFairies.Clear();
            PlayerUnits.Instance.OwnedFairies.AddRange(CombatPrepData.BackupOwnedFairies);

            // Restore selected allies for the retry
            CombatPrepData.SelectedAllies = new List<FairyData>(CombatPrepData.BackupSelectedAllies);

            if (consecutiveLosses == 2)
            {
                StartCoroutine(PlayTransitionAndShowGameOverScreen());
            }
            else
            {
                EnemyUnits.Instance.LoadStage(EnemyUnits.Instance.currentStageIndex);
            }
        }
        Time.timeScale = 1f;
    }
    private void ShowWinScreen()
    {
        SceneManager.LoadScene("WinScene");
    }
    private IEnumerator PlayTransitionAndShowGameOverScreen()
    {
        Time.timeScale = 1f;
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start");
            yield return new WaitForSeconds(1f); // Adjust to your animation's length
        }
        SceneManager.LoadScene("GameOverScene");
    }

    private void StartCombatEndSequence()
    {
        combatCanvasUI.enabled = true;
        RewardsManager.Instance.goldReward = GoldManager.Instance.gold - goldRewardHolder;
        goldRewardHolder = 0f;
        RewardsManager.Instance.DisplayRewards();
        StageManager.Instance.ToggleStageTextVisibility();
        speedCanvasButtonUI.enabled = false;
        skillCanvas.enabled = false;
    }
    //private IEnumerator SlowDownAndFreezeEffect()
    //{
    //    float targetTimeScale = 0.1f;
    //    float slowDuration = 0.5f;
    //    float freezeDuration = 4f;

    //    float startScale = Time.timeScale;
    //    float t = 0f;

    //    while (t < slowDuration)
    //    {
    //        t += Time.unscaledDeltaTime;
    //        Time.timeScale = Mathf.Lerp(startScale, targetTimeScale, t / slowDuration);
    //        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    //        yield return null;
    //    }
    //    Time.timeScale = targetTimeScale;
    //    Time.fixedDeltaTime = 0.02f * Time.timeScale;

    //    yield return new WaitForSecondsRealtime(freezeDuration);

    //    Time.timeScale = 0f;
    //    Time.fixedDeltaTime = 0.02f;
    //}

    #endregion

    #region UI & Utility

    public void ContinueToNextScene()
    {
        StartCoroutine(PlayTransitionAndLoadScene());
    }

    private IEnumerator PlayTransitionAndLoadScene()
    {
        Time.timeScale = 1f;
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start");
            // Wait for the transition animation to finish (adjust the time to your animation's length)
            yield return new WaitForSeconds(1f);
        }
        SceneManager.LoadScene("WaitingRoom");
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
    private void EnableArenaForCurrentStage()
    {
        // Disable all arenas first
        foreach (var arena in arenas)
        {
            if (arena != null)
                arena.SetActive(false);
        }

        // Enable the arena for the current stage
        int stageIndex = (int)EnemyUnits.Instance.currentStageIndex;
        if (stageIndex >= 0 && stageIndex < arenas.Length && arenas[stageIndex] != null)
        {
            arenas[stageIndex].SetActive(true);
        }
    }


    #endregion
}