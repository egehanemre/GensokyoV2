using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI stageText2;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        UpdateStageUI();
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
    }

    public void ToggleStageTextVisibility()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
