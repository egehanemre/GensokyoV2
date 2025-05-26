using UnityEngine;

[System.Serializable]
public class FairyData
{
    public string UniqueId; 
    public GameObject FairyPrefab;

    public FairyData(string uniqueId, GameObject prefab)
    {
        UniqueId = uniqueId;
        FairyPrefab = prefab;
    }
}