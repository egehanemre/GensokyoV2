using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public GameObject stageManagerObject;
    public static StageManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI stageText2;

    private string lastSceneName = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        lastSceneName = SceneManager.GetActiveScene().name;
        PlayStageMusic();
    }

    private void Update()
    {
        UpdateStageUI();

        // Detect scene change to play correct music
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != lastSceneName)
        {
            lastSceneName = currentScene;
            PlayStageMusic();
        }
    }

    public void UpdateStageUI()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "WaitingRoom")
        {
            stageText.text = EnemyUnits.Instance.currentStageIndex.ToString();
            stageText.gameObject.SetActive(true);
            stageText2.gameObject.SetActive(false);
        }
        if (sceneName == "CombatScene")
        {
            stageText.gameObject.SetActive(false);
            stageText2.text = EnemyUnits.Instance.currentStageIndex.ToString();
            stageText2.gameObject.SetActive(true);
        }
        if (sceneName == "GameOverScene" || sceneName == "VictoryScene")
        {
            stageText.gameObject.SetActive(false);
            stageText2.gameObject.SetActive(false);
        }
    }

    public void ToggleStageTextVisibility()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    private void PlayStageMusic()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (MusicManager.Instance == null)
            return;

        float fadeDuration = 1f;

        if (sceneName == "WaitingRoom")
        {
            MusicManager.Instance.PlayMusic(MusicManager.Instance.prepMusic, true, fadeDuration);
        }
        else if (sceneName == "CombatScene")
        {
            int stage = (int)EnemyUnits.Instance.currentStageIndex;
            // Use the stage index to select the combat music from the list
            MusicManager.Instance.PlayCombatMusicByIndex(stage, fadeDuration);
        }
    }
}