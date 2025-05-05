using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        var playerMeleeFairies = playerFairy.Where(f => f.fairyType == FairyType.Melee).ToList();
        var playerRangedFairies = playerFairy.Where(f => f.fairyType == FairyType.Ranged).ToList();
        var enemyMeleeFairies = enemyFairy.Where(f => f.fairyType == FairyType.Melee).ToList();
        var enemyRangedFairies = enemyFairy.Where(f => f.fairyType == FairyType.Ranged).ToList();

        MatchFairiesByType(playerMeleeFairies, enemyMeleeFairies);
        MatchFairiesByType(playerRangedFairies, enemyRangedFairies);
        MatchFairiesByType(playerMeleeFairies, enemyRangedFairies);

        var remainingPlayerFairies = playerMeleeFairies.Concat(playerRangedFairies).ToList();
        var remainingEnemyFairies = enemyMeleeFairies.Concat(enemyRangedFairies).ToList();
        MatchFairiesByType(remainingPlayerFairies, remainingEnemyFairies);
    }
    private void MatchFairiesByType(List<Fairy> playerFairies, List<Fairy> enemyFairies)
    {
        while (playerFairies.Count > 0 && enemyFairies.Count > 0)
        {
            Fairy closestPlayer = null;
            Fairy closestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (var player in playerFairies)
            {
                foreach (var enemy in enemyFairies)
                {
                    float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPlayer = player;
                        closestEnemy = enemy;
                    }
                }
            }

            if (closestPlayer != null && closestEnemy != null)
            {
                var playerTrackSystem = closestPlayer.GetComponent<TrackSystem>();
                var enemyTrackSystem = closestEnemy.GetComponent<TrackSystem>();

                if (playerTrackSystem != null && enemyTrackSystem != null)
                {
                    playerTrackSystem.Match = closestEnemy.gameObject;
                    enemyTrackSystem.Match = closestPlayer.gameObject;
                }

                playerFairies.Remove(closestPlayer);
                enemyFairies.Remove(closestEnemy);
            }
        }
    }
}
