using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public void MatchFairies()
    {
        var playerMelee = playerFairy.Where(f => f.fairyType == FairyType.Melee).ToList();
        var playerRanged = playerFairy.Where(f => f.fairyType == FairyType.Ranged).ToList();
        var enemyMelee = enemyFairy.Where(f => f.fairyType == FairyType.Melee).ToList();
        var enemyRanged = enemyFairy.Where(f => f.fairyType == FairyType.Ranged).ToList();

        MatchFairiesByType(playerMelee, enemyMelee);
        MatchFairiesByType(playerRanged, enemyRanged);
        MatchFairiesByType(playerMelee, enemyRanged);

        var remainingPlayers = playerMelee.Concat(playerRanged).ToList();
        var remainingEnemies = enemyMelee.Concat(enemyRanged).ToList();
        MatchFairiesByType(remainingPlayers, remainingEnemies);

        unmatchedPlayerFairies.AddRange(remainingPlayers);
        unmatchedEnemyFairies.AddRange(remainingEnemies);
    }
    public void AttemptInstantMatch(Fairy fairy)
{
    if (fairy == null)
        return;

    var opposingList = playerFairy.Contains(fairy) ? unmatchedEnemyFairies : unmatchedPlayerFairies;

    // Clean up nulls and dead units from the opposing list
    opposingList.RemoveAll(e => e == null || e.gameObject == null || e == fairy || e.Team == fairy.Team);

    Debug.Log($"[MatchingManager] {fairy.name} ({fairy.fairyType}) attempting instant match. Opposing unmatched list:");
    foreach (var opp in opposingList)
    {
        if (opp != null)
            Debug.Log($"    - {opp.name} ({opp.fairyType}) [{opp.Team}]");
    }

    if (opposingList.Count > 0)
    {
        // Exclude self and same team from matching
        var sameTypeMatches = opposingList
            .Where(e => e != null && e.fairyType == fairy.fairyType && e != fairy && e.Team != fairy.Team)
            .ToList();

        Fairy closest = null;

        if (sameTypeMatches.Count > 0)
        {
            closest = sameTypeMatches
                .OrderBy(e => Vector3.Distance(fairy.transform.position, e.transform.position))
                .FirstOrDefault();
        }
        else
        {
            closest = opposingList
                .Where(e => e != null && e != fairy && e.Team != fairy.Team)
                .OrderBy(e => Vector3.Distance(fairy.transform.position, e.transform.position))
                .FirstOrDefault();
        }

        if (closest != null && fairy != null)
        {
            var fairyTrack = fairy.GetComponent<TrackSystem>();
            var closestTrack = closest.GetComponent<TrackSystem>();
            if (fairyTrack != null && closestTrack != null)
            {
                Debug.Log($"[MatchingManager] {fairy.name} ({fairy.fairyType}) [{fairy.Team}] matched with {closest.name} ({closest.fairyType}) [{closest.Team}]");
                fairyTrack.Match = closest.gameObject;
                closestTrack.Match = fairy.gameObject;

                opposingList.Remove(closest);
                RemoveFromUnmatchedList(fairy);
            }
        }
        else
        {
            Debug.Log($"[MatchingManager] {fairy.name} ({fairy.fairyType}) found no available match.");
        }
    }
    else
    {
        Debug.Log($"[MatchingManager] {fairy.name} ({fairy.fairyType}) - opposing list empty, adding to unmatched.");
        AddToUnmatchedList(fairy);
    }
}
    private void AddToUnmatchedList(Fairy fairy)
    {
        if (playerFairy.Contains(fairy) && !unmatchedPlayerFairies.Contains(fairy))
            unmatchedPlayerFairies.Add(fairy);
        else if (enemyFairy.Contains(fairy) && !unmatchedEnemyFairies.Contains(fairy))
            unmatchedEnemyFairies.Add(fairy);
    }
    public void RemoveFromUnmatchedList(Fairy fairy)
    {
        unmatchedPlayerFairies.Remove(fairy);
        unmatchedEnemyFairies.Remove(fairy);
    }
    private void MatchFairiesByType(List<Fairy> playerList, List<Fairy> enemyList)
    {
        while (playerList.Count > 0 && enemyList.Count > 0)
        {
            Fairy bestPlayer = null, bestEnemy = null;
            float bestDist = float.MaxValue;

            foreach (var p in playerList)
            {
                foreach (var e in enemyList)
                {
                    float dist = Vector3.Distance(p.transform.position, e.transform.position);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestPlayer = p;
                        bestEnemy = e;
                    }
                }
            }

            if (bestPlayer != null && bestEnemy != null)
            {
                bestPlayer.GetComponent<TrackSystem>().Match = bestEnemy.gameObject;
                bestEnemy.GetComponent<TrackSystem>().Match = bestPlayer.gameObject;

                playerList.Remove(bestPlayer);
                enemyList.Remove(bestEnemy);
            }
        }
    }
    
}
