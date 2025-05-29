using UnityEngine;

public class DeadFairyDisplayManager : MonoBehaviour
{
    public static DeadFairyDisplayManager Instance { get; private set; }
    public GameObject deadFairyDisplayPrefab;
    public Transform displayParent; 
    private void Awake()
    {
        Instance = this;
    }

    public void ShowDeadFairy(FairyData fairyData)
    {
        var displayGO = Instantiate(deadFairyDisplayPrefab, displayParent);
        var display = displayGO.GetComponent<DeadFairyDisplay>();
        display.fairyData = fairyData;
        display.Init();
        display.fairyDisplay.SetActive(true);
    }
}