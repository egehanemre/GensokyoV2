using System.Collections.Generic;
using UnityEngine;

public class PlayerUnits : MonoBehaviour
{
    public List<GameObject> startingFairies = new List<GameObject>();
    public static PlayerUnits Instance { get; private set; }

    [SerializeField] private List<FairyData> ownedFairies = new List<FairyData>();
    public List<FairyData> OwnedFairies => ownedFairies;

    public const int MaxUnits = 6;

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
    }

    public bool AddFairy(GameObject fairyPrefab)
    {
        if (ownedFairies.Count >= MaxUnits)
            return false; 

        string uniqueId = System.Guid.NewGuid().ToString();
        ownedFairies.Add(new FairyData(uniqueId, fairyPrefab));
        fairyPrefab.GetComponent<Fairy>().UniqueId = uniqueId;
        return true;
    }

    public void RemoveFairy(string uniqueId)
    {
        ownedFairies.RemoveAll(f => f.UniqueId == uniqueId);
    }
    public float GetTotalOwnedFairiesPrice()
    {
        float total = 0f;
        foreach (var fairyData in ownedFairies)
        {
            if (fairyData.FairyPrefab != null)
            {
                var fairy = fairyData.FairyPrefab.GetComponent<Fairy>();
                if (fairy != null)
                {
                    total += fairy.price;
                }
            }
        }
        return total;
    }

    public void ResetGame()
    {
        ownedFairies.Clear();
        InitializeStartingFairies();
    }
    public void RestoreFromBackup()
    {
        if (CombatPrepData.BackupOwnedFairies != null && CombatPrepData.BackupOwnedFairies.Count > 0)
        {
            ownedFairies.Clear();
            ownedFairies.AddRange(CombatPrepData.BackupOwnedFairies);
        }
    }
}