using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatPrepManager : MonoBehaviour
{
    [SerializeField] private PlayerUnits playerUnits;
    [SerializeField] private GameObject prepUnitPrefab;
    [SerializeField] private GameObject prepUnitsParent;
    [SerializeField] private GameObject prepEnemiesParent;
    [SerializeField] private Button prepButton;

    public static int requiredFairyCount = 4;
    private void OnEnable()
    {
        RefreshPrepScreen();
    }
    private void Start()
    {
        if (prepButton != null)
        {
            prepButton.onClick.AddListener(OnPrepButtonClicked);
        }
    }
    public void RefreshPrepScreen()
    {
        requiredFairyCount = EnemyUnits.Instance.currentStageIndex switch
        {
            Stages.Stage1 => 4,
            Stages.Stage2 => 5,
            Stages.Stage3 => 6,
            Stages.Stage4 => 7,
            _ => 4
        };
        playerUnits = PlayerUnits.Instance;
        FairyShop.SelectedShops.Clear();
        UpdatePrepUnits();
        UpdatePrepEnemies();
        UpdateButtonText();
    }
    private void Update()
    {
        if (prepButton != null)
        {
            UpdateButtonText();
            var selectedCount = FairyShop.SelectedShops.Count;
            prepButton.interactable = selectedCount >= requiredFairyCount;
        }
    }
    private void UpdatePrepUnits()
    {
        foreach (Transform child in prepUnitsParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (FairyData fairyData in playerUnits.OwnedFairies)
        {
            GameObject prepEntry = Instantiate(prepUnitPrefab, prepUnitsParent.transform);
            FairyShop fairyShop = prepEntry.GetComponent<FairyShop>();

            if (fairyShop != null)
            {
                fairyShop.fairyData = fairyData; // Pass the data, not just the prefab
                fairyShop.shopType = FairyShop.ShopType.Prep;
                fairyShop.Init();
            }
        }
    }
    private void UpdatePrepEnemies()
    {
        foreach (Transform child in prepEnemiesParent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (GameObject enemyGO in EnemyUnits.Instance.EnemyFairies)
        {
            GameObject prepEntry = Instantiate(prepUnitPrefab, prepEnemiesParent.transform);
            FairyShop fairyShop = prepEntry.GetComponent<FairyShop>();
            if (fairyShop != null)
            {
                fairyShop.fairyData = new FairyData("Enemy_" + enemyGO.GetInstanceID(), enemyGO);
                fairyShop.shopType = FairyShop.ShopType.Enemy;
                fairyShop.Init();
            }
        }
    }
    public void UpdateButtonText()
    {
        var selectedFairies = FairyShop.SelectedShops.Select(shop => shop.fairyData).Where(fd => fd != null).ToList();

        prepButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start Combat (" + selectedFairies.Count + "/" + requiredFairyCount + ")";
    }
    private void OnPrepButtonClicked()
    {
        if (FairyShop.SelectedShops.Count >= requiredFairyCount)
        {
            CombatPrepData.SelectedAllies = FairyShop.SelectedShops
                .Select(shop => shop.fairyData)
                .Where(fd => fd != null)
                .ToList();

            CombatPrepData.SelectedEnemies = EnemyUnits.Instance.EnemyFairies
                .Select(enemyGO => new FairyData("Enemy_" + enemyGO.GetInstanceID(), enemyGO))
                .ToList();

            SceneManager.LoadScene("CombatScene");
        }
    }
}
