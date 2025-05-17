using UnityEngine;

public class MoveState : FairyState
{
    public MoveState(Fairy fairy) : base(fairy) { }
    private GameObject targetFairy;

    public override void Enter()
    {
        targetFairy = fairy.TrackSystem.Target;

        fairy.Animator.SetFloat("moveSpeed", fairy.fairyCurrentStats.moveSpeed);
    }
    public override void Update()
    {
        if (fairy.IsStunned || fairy.IsBlocking || fairy.IsDodging)
        {
            fairy.ChangeState(new IdleState(fairy));
            return;
        }

        if (fairy.TrackSystem.Target == null)
        {
            fairy.ChangeState(new IdleState(fairy));
            return;
        }

        var targetFairy = fairy.TrackSystem.Target.GetComponent<Fairy>();
        if (targetFairy == null)
        {
            fairy.ChangeState(new IdleState(fairy));
            return;
        }

        float distance = Vector3.Distance(fairy.transform.position, targetFairy.transform.position);

        if (distance > fairy.fairyCurrentStats.attackRange)
        {
            // Move towards target
            Vector3 dir = (targetFairy.transform.position - fairy.transform.position).normalized;
            Vector3 newPos = fairy.Rigidbody.position + dir * fairy.fairyCurrentStats.moveSpeed * Time.deltaTime;
            fairy.Rigidbody.MovePosition(newPos);

            Quaternion targetRot = Quaternion.LookRotation(dir);
            fairy.transform.rotation = Quaternion.Slerp(fairy.transform.rotation, targetRot, Time.deltaTime * 5f);
        }
        else
        {
            // Only enter AttackState if not on cooldown
            if (fairy.CanAttack)
                fairy.ChangeState(new AttackState(fairy));
            else
                fairy.ChangeState(new IdleState(fairy)); // Or remain in MoveState, or play a waiting animation
        }
    }

    public override void Exit()
    {
        fairy.Animator.SetFloat("moveSpeed", 0f);
    }
    private void MoveTowardsTarget()
    {
        Vector3 dir = (targetFairy.transform.position - fairy.transform.position).normalized;
        Vector3 newPos = fairy.Rigidbody.position + dir * fairy.fairyCurrentStats.moveSpeed * Time.deltaTime;
        fairy.Rigidbody.MovePosition(newPos);
    }
}
