using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GoldManager : MonoBehaviour
{
    public static float startingGold = 50f; // Initial gold amount
    public static GoldManager Instance { get; private set; }

    [SerializeField] public float gold;
    [SerializeField] public TextMeshProUGUI goldText;
    public Canvas goldCanvas;
    public float Gold => gold;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        gold = startingGold;
        UpdateGoldUI();
        UpdateGoldCanvasVisibility();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateGoldCanvasVisibility();
    }

    private void UpdateGoldCanvasVisibility()
    {
        if (goldCanvas != null)
        {
            goldCanvas.enabled = SceneManager.GetActiveScene().name != "MainMenu";
        }
    }

    private void ClampGoldToOneDecimal()
    {
        gold = Mathf.Floor(gold * 10f) / 10f;
    }
    public void AddGold(float amount)
    {
        gold += amount;
        ClampGoldToOneDecimal();
        UpdateGoldUI();
    }
    public void SpendGold(float amount)
    {
        gold -= amount;
        if (gold < 0) gold = 0;
        ClampGoldToOneDecimal();
        UpdateGoldUI();
    }

    public void SetGold(float amount)
    {
        gold = amount;
        ClampGoldToOneDecimal();
        UpdateGoldUI();
    }

    public float GetGold()
    {
        return gold;
    }

    public void SetGoldText(TextMeshProUGUI newGoldText)
    {
        goldText = newGoldText;
        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = gold.ToString();
    }
    public void ResetGame()
    {
        SetGold(startingGold);
    }
}