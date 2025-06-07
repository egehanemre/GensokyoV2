using System.Collections.Generic;
using UnityEngine;

public class UnitsForSale : MonoBehaviour
{
    public static UnitsForSale Instance { get; private set; }

    [Header("All possible units that can be sold (set in Inspector)")]
    [SerializeField] private List<GameObject> allUnitsForSale = new List<GameObject>();

    [Header("Units for sale this stage (auto-filled)")]
    [SerializeField] private List<GameObject> forSaleFairies = new List<GameObject>();
    public List<GameObject> ForSaleFairies => forSaleFairies;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        PickRandomUnitsForStage();
    }

    public void PickRandomUnitsForStage(int count = 3)
    {
        forSaleFairies.Clear();
        if (allUnitsForSale.Count <= count)
        {
            forSaleFairies.AddRange(allUnitsForSale);
            return;
        }

        List<GameObject> pool = new List<GameObject>(allUnitsForSale);
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, pool.Count);
            forSaleFairies.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
    }
}