using System.Collections.Generic;
using UnityEngine;

public class PlayerUnits : MonoBehaviour
{
    public List<GameObject> startingFairies = new List<GameObject>();
    public static PlayerUnits Instance { get; private set; }

    [SerializeField] private List<FairyData> ownedFairies = new List<FairyData>();
    public List<FairyData> OwnedFairies => ownedFairies;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        InitializeStartingFairies();
    }

    public void InitializeStartingFairies()
    {
        foreach (GameObject fairyPrefab in startingFairies)
        {
            if (fairyPrefab != null)
            {
                string uniqueId = System.Guid.NewGuid().ToString();
                ownedFairies.Add(new FairyData(uniqueId, fairyPrefab));
            }
        }
        startingFairies.Clear();
    }

    public void AddFairy(GameObject fairyPrefab)
    {
        string uniqueId = System.Guid.NewGuid().ToString();
        ownedFairies.Add(new FairyData(uniqueId, fairyPrefab));
        fairyPrefab.GetComponent<Fairy>().UniqueId = uniqueId;
    }

    public void RemoveFairy(string uniqueId)
    {
        ownedFairies.RemoveAll(f => f.UniqueId == uniqueId);
    }
}