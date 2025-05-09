using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchingManager : MonoBehaviour
{
    public static MatchingManager Instance;
    
    public List<Fairy> playerFairy;
    public List<Fairy> enemyFairy;

    private List<Fairy> unmatchedPlayerFairies = new List<Fairy>();
    private List<Fairy> unmatchedEnemyFairies = new List<Fairy>();
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        foreach (var fairy in playerFairy)
        {
            fairy.Team = 0; // Player team
        }

        foreach (var fairy in enemyFairy)
        {
            fairy.Team = 1; // Enemy team
        }

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

        unmatchedPlayerFairies.AddRange(remainingPlayerFairies);
        unmatchedEnemyFairies.AddRange(remainingEnemyFairies);
    }
    public void AttemptInstantMatch(Fairy fairy)
    {
        List<Fairy> opposingUnmatchedList = playerFairy.Contains(fairy) ? unmatchedEnemyFairies : unmatchedPlayerFairies;

        if (opposingUnmatchedList.Count > 0)
        {
            var closestFairy = opposingUnmatchedList
                .OrderBy(enemy => Vector3.Distance(fairy.transform.position, enemy.transform.position))
                .FirstOrDefault();

            if (closestFairy != null)
            {
                var fairyTrackSystem = fairy.GetComponent<TrackSystem>();
                var enemyTrackSystem = closestFairy.GetComponent<TrackSystem>();

                if (fairyTrackSystem != null && enemyTrackSystem != null)
                {
                    fairyTrackSystem.Match = closestFairy.gameObject;
                    enemyTrackSystem.Match = fairy.gameObject;

                    opposingUnmatchedList.Remove(closestFairy);
                    RemoveFromUnmatchedList(fairy);
                }
            }
        }
        else
        {
            AddToUnmatchedList(fairy);
        }
    }
    private void AddToUnmatchedList(Fairy fairy)
    {
        if (playerFairy.Contains(fairy) && !unmatchedPlayerFairies.Contains(fairy))
        {
            unmatchedPlayerFairies.Add(fairy);
        }
        else if (enemyFairy.Contains(fairy) && !unmatchedEnemyFairies.Contains(fairy))
        {
            unmatchedEnemyFairies.Add(fairy);
        }
    }
    private void RemoveFromUnmatchedList(Fairy fairy)
    {
        unmatchedPlayerFairies.Remove(fairy);
        unmatchedEnemyFairies.Remove(fairy);
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
