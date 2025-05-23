using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FairyShop : MonoBehaviour
{
    public GameObject fairyPrefab;
    public GameObject fairyDisplay;
    public GameObject objectParent;

    public ShopType shopType;
    private FairyStatsSO fairyStatsBase; 
    private WeaponDataSO weaponDataSO;
    public float price;


    [SerializeField] private TextMeshProUGUI Attack;
    [SerializeField] private TextMeshProUGUI Health;
    [SerializeField] private TextMeshProUGUI Speed;
    [SerializeField] private Button Button;

    private void Awake() {

    }

    
    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    public void Init()
    {
        if (fairyPrefab == null || objectParent == null) return;

        Fairy fairy = fairyPrefab.GetComponent<Fairy>();
        fairyStatsBase = fairy.fairyStatsBase;
        weaponDataSO = fairy.weaponDataSO;

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
        Health.text = "Health: " + fairyStatsBase.maxHealth.ToString();
        Attack.text = "Attack: " + weaponDataSO.attackDamage.ToString();
        Speed.text = "Speed: " + fairyStatsBase.moveSpeed.ToString();

        var buttonText = Button.GetComponentInChildren<TextMeshProUGUI>();
        
        Button.onClick.RemoveAllListeners();

        if (shopType == ShopType.Buy)
        {
            buttonText.text = "Buy " + price.ToString();
            Button.onClick.AddListener(() => BuyFairy(fairyPrefab));
        }
        else if (shopType == ShopType.Sell)
        {
            buttonText.text = "Sell " + (price / 2f).ToString();
            Button.onClick.AddListener(() => SellFairy(fairyPrefab));
        }
        else if (shopType == ShopType.Prep)
        {
            buttonText.text = "Select";
        }
    }
    public enum ShopType
    {
        Buy,
        Sell,
        Prep
    }
    public void BuyFairy(GameObject fairy)
    {
        PlayerUnits.Instance.AddFairy(fairy);
        Init();
    }
    public void SellFairy(GameObject fairy)
    {
        PlayerUnits.Instance.RemoveFairy(fairy);
        Init();
    }
}
