using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [SerializeField] private List<GameObject> playerSideEnemies;
    [SerializeField] private List<GameObject> enemySideEnemies;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void InitializeCombat()
    {
        Debug.Log("Initializing Combat...");
    }
}
