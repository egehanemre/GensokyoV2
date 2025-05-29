using System.Linq;
using UnityEngine;

public class TrackSystem : MonoBehaviour
{
    public GameObject Match;
    public GameObject Target;
    private Fairy fairy;

    private void Awake()
    {
        fairy = GetComponent<Fairy>();
    }
    public void TryMatch()
    {
        if (Match == null)
        {
            Debug.Log($"[TrackSystem] {fairy.name} ({fairy.fairyType}) is attempting to match.");
            MatchingManager.Instance?.AttemptInstantMatch(fairy);
        }
        if (Match != null)
        {
            Target = Match;
        }
        else
        {
            FindNewTarget();
        }
    }

    public void ClearMatch()
    {
        if (Match != null)
        {
            var enemyTrackSystem = Match.GetComponent<TrackSystem>();
            if (enemyTrackSystem != null)
            {
                enemyTrackSystem.Match = null;
            }

            Match = null;
        }
    }

    public void OnUnitDeath()
    {
        ClearMatch();

        if (MatchingManager.Instance != null)
        {
            MatchingManager.Instance.playerFairy.Remove(fairy);
            MatchingManager.Instance.enemyFairy.Remove(fairy);

            // Remove from unmatched lists as well
            MatchingManager.Instance.RemoveFromUnmatchedList(fairy);
        }

        NotifyTargetersToFindNewTarget();
    }

    private void NotifyTargetersToFindNewTarget()
    {
        var allFairies = MatchingManager.Instance.playerFairy.Concat(MatchingManager.Instance.enemyFairy).ToList();

        foreach (var otherFairy in allFairies)
        {
            var otherTrackSystem = otherFairy.GetComponent<TrackSystem>();
            if (otherTrackSystem != null && otherTrackSystem.Target == this.gameObject)
            {
                otherTrackSystem.FindNewTarget();
                otherTrackSystem.TryMatch(); // Only try to match when notified
            }
        }
    }

    public void FindNewTarget()
    {
        var potentialTargets = MatchingManager.Instance.enemyFairy.Contains(fairy)
            ? MatchingManager.Instance.playerFairy
            : MatchingManager.Instance.enemyFairy;

        var closestFairy = potentialTargets
            .Where(e => e != null && e.gameObject != this.gameObject)
            .OrderBy(e => Vector3.Distance(fairy.transform.position, e.transform.position))
            .FirstOrDefault();

        Target = closestFairy?.gameObject;
    }
}