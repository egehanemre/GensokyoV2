using System.Collections.Generic;
using UnityEngine;

public class UnitsForSale : MonoBehaviour 
{
    [SerializeField] private List<GameObject> forSaleFairies = new List<GameObject>();
    public List<GameObject> ForSaleFairies => forSaleFairies;
}
