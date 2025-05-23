using UnityEngine;

public class CombatPrepManager : MonoBehaviour
{
    [SerializeField] private PlayerUnits playerUnits;
    [SerializeField] private GameObject prepUnitPrefab;
    [SerializeField] private GameObject prepUnitsParent;

    private void Start()
    {
        UpdatePrepUnits();
    }
    private void UpdatePrepUnits()
    {
        foreach (Transform child in prepUnitsParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (GameObject fairyGO in playerUnits.OwnedFairies)
        {
            GameObject prepEntry = Instantiate(prepUnitPrefab, prepUnitsParent.transform);
            FairyShop fairyShop = prepEntry.GetComponent<FairyShop>();

            if (fairyShop != null)
            {
                fairyShop.fairyPrefab = fairyGO;
                fairyShop.shopType = FairyShop.ShopType.Prep;
                fairyShop.Init();
            }
        }
    }
}
