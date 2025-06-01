using UnityEngine;

public class MoveState : FairyState
{
    private float circleRadialBias;
    private CombatMoveBehavior currentBehavior;
    private float behaviorTimer;

    public MoveState(Fairy fairy) : base(fairy) { }

    public override void Enter()
    {
        ChooseNewBehavior();
        fairy.CurrentMoveSpeed = fairy.fairyCurrentStats.moveSpeed;
    }

    public override void Update()
    {
        var targetGO = fairy.TrackSystem.Target;
        if (targetGO == null)
        {
            fairy.ChangeState(new IdleState(fairy));
            return;
        }

        var targetFairy = targetGO.GetComponent<Fairy>();
        if (targetFairy == null)
        {
            fairy.ChangeState(new IdleState(fairy));
            return;
        }

        float distance = Vector3.Distance(fairy.transform.position, targetFairy.transform.position);

        if (distance > fairy.fairyCurrentStats.attackRange)
        {
            behaviorTimer -= Time.deltaTime;
            if (behaviorTimer <= 0f)
                ChooseNewBehavior();

            HandleMoveBehavior(targetFairy, distance);
        }
        else
        {
            if (fairy.IsAttackReady())
            {
                fairy.ChangeState(new AttackState(fairy));
            }
            else
            {
                behaviorTimer -= Time.deltaTime;
                if (behaviorTimer <= 0f)
                    ChooseNewBehavior();

                HandleMoveBehavior(targetFairy, distance);
            }
        }
    }
    private void HandleMoveBehavior(Fairy targetFairy, float distance)
    {
        Vector3 dirToTarget = (targetFairy.transform.position - fairy.transform.position).normalized;
        Vector3 move = Vector3.zero;

        bool isMatchedTarget = fairy.TrackSystem.IsTargetMatched(targetFairy); 

        switch (currentBehavior)
        {
            case CombatMoveBehavior.Direct:
                if (!isMatchedTarget)
                {
                    Vector3 sideCurve = Vector3.Cross(Vector3.up, dirToTarget).normalized;
                    float offset = Mathf.Sin(Time.time * 1.5f + fairy.GetHashCode()) * 0.3f;
                    move = (dirToTarget + sideCurve * offset).normalized;
                }
                else
                {
                    move = dirToTarget;
                }
                break;

            case CombatMoveBehavior.Circle:
                float desiredRadius = fairy.fairyCurrentStats.attackRange * 0.9f;
                float radiusDiff = distance - desiredRadius;

                if (fairy.IsAttackReady() && distance > fairy.fairyCurrentStats.attackRange)
                {
                    move = dirToTarget;
                }
                else
                {
                    Vector3 circleDir = Vector3.Cross(Vector3.up, dirToTarget).normalized;
                    Vector3 radialAdjust = dirToTarget * Mathf.Clamp(radiusDiff * 0.5f, -1f, 1f);
                    move = (circleDir + radialAdjust).normalized;
                }
                break;

            case CombatMoveBehavior.Retreat:
                if (distance < fairy.fairyCurrentStats.attackRange * 0.8f)
                    move = -dirToTarget;
                else
                    move = dirToTarget;
                break;

            case CombatMoveBehavior.Idle:
                move = Vector3.zero;
                break;
        }

        if (move != Vector3.zero)
        {
            Vector3 newPos = fairy.Rigidbody.position + move * fairy.CurrentMoveSpeed * Time.deltaTime;
            fairy.Rigidbody.MovePosition(newPos);

            Quaternion targetRot = Quaternion.LookRotation(dirToTarget);
            fairy.transform.rotation = Quaternion.Slerp(fairy.transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }
    public override void Exit()
    {
        fairy.CurrentMoveSpeed = 0f;
    }
    private enum CombatMoveBehavior
    {
        Direct,
        Circle,
        Retreat,
        Idle
    }
    private void ChooseNewBehavior()
    {
        int nearbyAllies = CountNearbyAllies();
        float circleChance = Mathf.Clamp01(0.5f + 0.05f * nearbyAllies);
        float retreatChance = 0.1f;
        float directChance = 0.4f;
        float rand = Random.value;

        if (rand < retreatChance)
        {
            currentBehavior = CombatMoveBehavior.Retreat;
            behaviorTimer = Random.Range(0.5f, 1.5f);
        }
        else if (rand < retreatChance + directChance)
        {
            currentBehavior = CombatMoveBehavior.Direct;
            behaviorTimer = Random.Range(1f, 2f);
        }
        else if (rand < 1f)
        {
            currentBehavior = CombatMoveBehavior.Circle;
            behaviorTimer = Random.Range(2f, 4f);
            circleRadialBias = Random.Range(-0.5f, 0.5f);
        }
        else
        {
            behaviorTimer = Random.Range(0.5f, 1f);
        }
    }
    private int CountNearbyAllies(float radius = 3f)
    {
        int count = 0;
        Collider[] colliders = Physics.OverlapSphere(fairy.transform.position, radius);
        foreach (var col in colliders)
        {
            if (col == null) continue;
            Fairy otherFairy = col.GetComponent<Fairy>();
            if (otherFairy != null && otherFairy != fairy && otherFairy.Team == fairy.Team)
                count++;
        }
        return count;
    }
}
