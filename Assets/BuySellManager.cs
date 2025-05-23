using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class BuySellManager : MonoBehaviour
{
    [SerializeField] private PlayerUnits playerUnits;
    [SerializeField] private UnitsForSale unitsForSale;
    [SerializeField] private GameObject shopPrefab;

    [SerializeField] private GameObject shopBuy;
    [SerializeField] private GameObject shopSell;

    [SerializeField] public float Gold = 50f;
    [SerializeField] private TextMeshProUGUI gold;


    private void Start()
    {
        UpdateShop();
    }
    private void Update()
    {
        gold.text = "Gold: " + Gold.ToString();
    }
    private void UpdateShop()
    {
        foreach (Transform child in shopSell.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (GameObject fairyGO in playerUnits.OwnedFairies)
        {
            GameObject shopEntry = Instantiate(shopPrefab, shopSell.transform);
            FairyShop fairyShop = shopEntry.GetComponent<FairyShop>();

            if (fairyShop != null)
            {
                fairyShop.fairyPrefab = fairyGO;
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
                fairyShop.fairyPrefab = fairyGO;
                fairyShop.shopType = FairyShop.ShopType.Buy;
                fairyShop.Init();
            }
        }
    }
}
