using UnityEngine;
using System.Collections.Generic;

public class MatchingManager : MonoBehaviour
{
    public static MatchingManager Instance;

    public List<Fairy> playerFairy;
    public List<Fairy> enemyFairy;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        MatchFairies();
    }

    public void MatchFairies()
    {
        ClearInvalidMatches();

        foreach (var player in playerFairy)
        {
            var playerTargeting = player.GetComponent<TargetingManager>();
            if (playerTargeting.Match != null) continue; 

            Fairy closestEnemy = FindClosestEnemy(player, enemyFairy, true);
            if (closestEnemy == null)
            {
                closestEnemy = FindClosestEnemy(player, enemyFairy, false);
            }

            if (closestEnemy != null)
            {
                SetMatch(player, closestEnemy);
            }
        }
    }
    private void ClearInvalidMatches()
    {
        foreach (var fairy in playerFairy)
        {
            var targeting = fairy.GetComponent<TargetingManager>();
            if (targeting.Match != null && !enemyFairy.Contains(targeting.Match.GetComponent<Fairy>()))
            {
                targeting.Match = null;
            }
        }

        foreach (var fairy in enemyFairy)
        {
            var targeting = fairy.GetComponent<TargetingManager>();
            if (targeting.Match != null && !playerFairy.Contains(targeting.Match.GetComponent<Fairy>()))
            {
                targeting.Match = null;
            }
        }
    }
    private void SetMatch(Fairy player, Fairy enemy)
    {
        var playerTargeting = player.GetComponent<TargetingManager>();
        var enemyTargeting = enemy.GetComponent<TargetingManager>();

        playerTargeting.Match = enemy.gameObject;
        playerTargeting.Target = enemy.gameObject;

        enemyTargeting.Match = player.gameObject;
        enemyTargeting.Target = player.gameObject;

        Debug.Log($"{player.name} is matched with {enemy.name}");
    }
    public Fairy FindClosestEnemy(Fairy player, List<Fairy> enemies, bool prioritizeMelee)
    {
        Fairy closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            var enemyTargeting = enemy.GetComponent<TargetingManager>();
            if (enemyTargeting.Match != null) continue; 
            if (prioritizeMelee && enemy.weaponDataSO.weaponType != WeaponDataSO.WeaponType.Melee) continue;
            if (!prioritizeMelee && enemy.weaponDataSO.weaponType != WeaponDataSO.WeaponType.Ranged) continue;

            float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    public void RemoveFairy(Fairy fairy)
    {
        if (playerFairy.Contains(fairy))
        {
            playerFairy.Remove(fairy);
        }
        else if (enemyFairy.Contains(fairy))
        {
            enemyFairy.Remove(fairy);
        }

        foreach (var player in playerFairy)
        {
            var targeting = player.GetComponent<TargetingManager>();
            if (targeting.Match == fairy.gameObject)
            {
                targeting.Match = null;
            }
            if (targeting.Target == fairy.gameObject)
            {
                targeting.Target = null;
            }
        }

        foreach (var enemy in enemyFairy)
        {
            var targeting = enemy.GetComponent<TargetingManager>();
            if (targeting.Match == fairy.gameObject)
            {
                targeting.Match = null;
            }
            if (targeting.Target == fairy.gameObject)
            {
                targeting.Target = null;
            }
        }
        MatchFairies();
    }
}
