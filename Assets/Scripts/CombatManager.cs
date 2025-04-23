using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    [SerializeField] private List<Enemy> playerSideEnemies = new List<Enemy>();
    [SerializeField] private List<Enemy> enemySideEnemies = new List<Enemy>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterPlayerSideEnemy(Enemy enemy)
    {
        if (!playerSideEnemies.Contains(enemy))
        {
            playerSideEnemies.Add(enemy);
        }
    }

    public void RegisterEnemySideEnemy(Enemy enemy)
    {
        if (!enemySideEnemies.Contains(enemy))
        {
            enemySideEnemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        playerSideEnemies.Remove(enemy);
        enemySideEnemies.Remove(enemy);
    }

    public void InitializeCombat()
    {
        Debug.Log("Initializing Combat...");
        AssignTargets();
    }

    private void AssignTargets()
    {
        Debug.Log("Assigning targets for all allies and enemies...");

        // Assign all enemies as targets for each player-side enemy
        foreach (Enemy playerSideEnemy in playerSideEnemies)
        {
            foreach (Enemy enemySideEnemy in enemySideEnemies)
            {
                playerSideEnemy.Target = enemySideEnemy.transform; // Example: Assign the first enemy as the primary target
                Debug.Log($"{playerSideEnemy.gameObject.name} targets {enemySideEnemy.gameObject.name}");
            }
        }

        // Assign all allies as targets for each enemy-side enemy
        foreach (Enemy enemySideEnemy in enemySideEnemies)
        {
            foreach (Enemy playerSideEnemy in playerSideEnemies)
            {
                enemySideEnemy.Target = playerSideEnemy.transform; // Example: Assign the first ally as the primary target
                Debug.Log($"{enemySideEnemy.gameObject.name} targets {playerSideEnemy.gameObject.name}");
            }
        }
    }

}
