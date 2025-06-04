using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FairyShop : MonoBehaviour
{
    public enum ShopType
    {
        Buy,
        Sell,
        Prep,
        Enemy
    }

    public FairyData fairyData; // For prep mode, holds the data instead of the prefab
    public GameObject fairyDisplay;
    public GameObject objectParent;
    public ShopType shopType;

    private FairyStatsSO fairyStatsBase;
    private WeaponDataSO weaponDataSO;
    private float buyPrice;
    private float sellPrice;

    [SerializeField] private TextMeshProUGUI attack;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI moveSpeed;
    [SerializeField] private TextMeshProUGUI range;
    [SerializeField] private TextMeshProUGUI attackSpeed;
    [SerializeField] private TextMeshProUGUI defense;
    [SerializeField] private Button button;

    private bool isSelected = false;
    public static HashSet<FairyShop> SelectedShops { get; } = new HashSet<FairyShop>();
    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    public void Init()
    {
        if (fairyData == null || fairyData.FairyPrefab == null || objectParent == null) return;

        if (!fairyData.FairyPrefab.TryGetComponent<Fairy>(out var fairy)) return;

        fairyStatsBase = fairy.fairyStatsBase;
        weaponDataSO = fairy.weaponDataSO;
        buyPrice = fairy.price;
        sellPrice = buyPrice / 2f;

        if (fairyDisplay != null)
            Destroy(fairyDisplay);

        if (fairy.fairyImageForShop != null)
        {
            fairyDisplay = Instantiate(fairy.fairyImageForShop, objectParent.transform);
            fairyDisplay.transform.localPosition = Vector3.zero;
            fairyDisplay.transform.localRotation = Quaternion.identity;
            fairyDisplay.transform.localScale = Vector3.one;

            int uiLayer = LayerMask.NameToLayer("UI");
            if (uiLayer != -1)
                SetLayerRecursively(fairyDisplay, uiLayer);
        }

        health.text = fairyStatsBase.maxHealth.ToString();
        moveSpeed.text = fairyStatsBase.moveSpeed.ToString();
        range.text = weaponDataSO.attackRange.ToString();
        attackSpeed.text = (1f / weaponDataSO.attackCooldown).ToString("F2");
        defense.text = fairyStatsBase.defense.ToString();
        attack.text = weaponDataSO.attackDamage.ToString();

        var buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        button.onClick.RemoveAllListeners();

        switch (shopType)
        {
            case ShopType.Buy:
                buttonText.text = $"Buy {buyPrice}";
                button.onClick.AddListener(() => BuyFairy(fairyData.FairyPrefab));
                button.gameObject.SetActive(true);
                break;
            case ShopType.Sell:
                buttonText.text = $"Sell {sellPrice}";
                button.onClick.AddListener(() => SellFairy(fairyData.UniqueId));
                button.gameObject.SetActive(true);
                break;
            case ShopType.Prep:
                buttonText.text = "Select";
                button.onClick.AddListener(ToggleSelect);
                button.gameObject.SetActive(true);
                UpdateButtonColor();
                break;
            case ShopType.Enemy:
                button.gameObject.SetActive(false);
                break;
        }
    }
    private void ToggleSelect()
    {
        if (isSelected)
        {
            isSelected = false;
            SelectedShops.Remove(this);
        }
        else
        {
            if (SelectedShops.Count >= CombatPrepManager.requiredFairyCount)
                return;
            isSelected = true;
            SelectedShops.Add(this);
        }
        UpdateButtonColor();
    }
    private void UpdateButtonColor()
    {
        var colors = button.colors;
        if (isSelected)
        {
            colors.normalColor = Color.green;
            colors.selectedColor = Color.green;
        }
        else
        {
            colors.normalColor = Color.white;
            colors.selectedColor = Color.white;
        }
        button.colors = colors;
    }
    public void BuyFairy(GameObject fairyPrefab)
    {
        if (GoldManager.Instance.Gold < buyPrice) return;

        PlayerUnits.Instance.AddFairy(fairyPrefab);
        GoldManager.Instance.SpendGold(buyPrice);
        UnitsForSale.Instance.ForSaleFairies.Remove(fairyPrefab);
        BuySellManager.Instance.UpdateShop();
    }
    public void SellFairy(string uniqueId)
    {
        if (PlayerUnits.Instance.OwnedFairies.Count == 1) return;

        GoldManager.Instance.AddGold(sellPrice);
        PlayerUnits.Instance.RemoveFairy(uniqueId);
        Destroy(gameObject);
        BuySellManager.Instance.UpdateShop();
    }
}