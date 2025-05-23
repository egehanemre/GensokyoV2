using System.Collections.Generic;
using UnityEngine;

public class PlayerUnits : MonoBehaviour
{
    public static PlayerUnits Instance { get; private set; }

    [SerializeField] private List<GameObject> ownedFairies = new List<GameObject>();
    public List<GameObject> OwnedFairies => ownedFairies;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    public void AddFairy(GameObject fairy)
    {
        ownedFairies.Add(fairy);
    }
    public void RemoveFairy(GameObject fairy)
    {
        if (fairy != null && ownedFairies.Contains(fairy))
        {
            ownedFairies.Remove(fairy);
        }
    }
}
