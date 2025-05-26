using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class BuySellManager : MonoBehaviour
{
    private List<GameObject> shopBuyEntries = new();
    private List<GameObject> shopSellEntries = new();
    public static BuySellManager Instance { get; private set; }

    [SerializeField] private PlayerUnits playerUnits;
    [SerializeField] private UnitsForSale unitsForSale;
    [SerializeField] private GameObject shopPrefab;

    [SerializeField] private GameObject shopBuy;
    [SerializeField] private GameObject shopSell;

    private float Gold;

    private void Awake()
    {
        playerUnits = PlayerUnits.Instance;

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateShop();
    }
    private void Update()
    {
    }
    public void UpdateShop()
    {
        foreach (Transform child in shopSell.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (FairyData fairyData in playerUnits.OwnedFairies)
        {
            GameObject shopEntry = Instantiate(shopPrefab, shopSell.transform);
            FairyShop fairyShop = shopEntry.GetComponent<FairyShop>();

            if (fairyShop != null)
            {
                fairyShop.fairyData = fairyData;
                fairyShop.shopType = FairyShop.ShopType.Sell;
                fairyShop.Init();
            }
        }
        foreach (Transform child in shopBuy.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (GameObject fairyGO in unitsForSale.ForSaleFairies)
        {
            GameObject shopEntry = Instantiate(shopPrefab, shopBuy.transform);
            FairyShop fairyShop = shopEntry.GetComponent<FairyShop>();

            if (fairyShop != null)
            {
                fairyShop.fairyData.FairyPrefab = fairyGO;
                fairyShop.shopType = FairyShop.ShopType.Buy;
                fairyShop.Init();
            }
        }
    }
}