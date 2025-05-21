using UnityEngine;

public class MoveState : FairyState
{
    public MoveState(Fairy fairy) : base(fairy) { }

    public override void Enter()
    {
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
            Vector3 dir = (targetFairy.transform.position - fairy.transform.position).normalized;
            Vector3 newPos = fairy.Rigidbody.position + dir * fairy.CurrentMoveSpeed * Time.deltaTime;
            fairy.Rigidbody.MovePosition(newPos);

            Quaternion targetRot = Quaternion.LookRotation(dir);
            fairy.transform.rotation = Quaternion.Slerp(fairy.transform.rotation, targetRot, Time.deltaTime * 5f);
        }
        else
        {
            fairy.ChangeState(new AttackState(fairy));
        }
    }

    public override void Exit()
    {
        fairy.CurrentMoveSpeed = 0f;
    }
}
