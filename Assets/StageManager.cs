using TMPro;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI stageText;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        UpdateStageUI();
    }

    public void UpdateStageUI()
    {
        if (stageText != null)
        {
            stageText.text = "Stage: " + EnemyUnits.Instance.currentStageIndex ;
        }
    }
}
