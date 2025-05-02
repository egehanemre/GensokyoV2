using UnityEngine;

public class TargetingManager : MonoBehaviour
{
    public GameObject Match;
    public GameObject Target; 

    private Fairy fairy;

    private void Awake()
    {
        fairy = GetComponent<Fairy>();
    }

    private void Update()
    {
        if (Match == null || IsFairyDead(Match))
        {
            Match = null;
        }

        if (Target == null || IsFairyDead(Target))
        {
            FindNewTarget();
        }
    }

    private void FindNewTarget()
    {
        Fairy newTarget = MatchingManager.Instance.FindClosestEnemy(fairy, MatchingManager.Instance.enemyFairy, true);
        if (newTarget != null)
        {
            Target = newTarget.gameObject;
            return;
        }

        newTarget = MatchingManager.Instance.FindClosestEnemy(fairy, MatchingManager.Instance.enemyFairy, false);
        if (newTarget != null)
        {
            Target = newTarget.gameObject;
            return;
        }

        Target = null;
    }

    private bool IsFairyDead(GameObject fairyObject)
    {
        if (fairyObject.TryGetComponent<Fairy>(out Fairy fairy))
        {
            return fairy.fairyCurrentStats.currentHealth <= 0;
        }
        return true;
    }
}
