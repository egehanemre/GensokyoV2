using UnityEngine;
using TMPro;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    [SerializeField] private float gold = 50f;
    [SerializeField] private TextMeshProUGUI goldText;
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
        UpdateGoldUI();
    }

    public void AddGold(float amount)
    {
        gold += amount;
        UpdateGoldUI();
    }

    public void SpendGold(float amount)
    {
        gold -= amount;
        if (gold < 0) gold = 0;
        UpdateGoldUI();
    }

    public void SetGold(float amount)
    {
        gold = amount;
        UpdateGoldUI();
    }

    public void SetGoldText(TextMeshProUGUI newGoldText)
    {
        goldText = newGoldText;
        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = "Gold: " + gold.ToString();
    }
}