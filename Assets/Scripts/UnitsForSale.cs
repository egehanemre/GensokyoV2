using System.Collections.Generic;
using UnityEngine;

public class UnitsForSale : MonoBehaviour
{
    public static UnitsForSale Instance { get; private set; }

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
    }
}