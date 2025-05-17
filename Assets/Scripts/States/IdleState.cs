using UnityEngine;

public class IdleState : FairyState
{
    public IdleState(Fairy fairy) : base(fairy) { }

    public override void Enter()
    {
        fairy.Animator.SetFloat("moveSpeed", 0f);
    }

    public override void Update()
    {
        if (fairy.TrackSystem.Target != null)
        {
            float dist = Vector3.Distance(fairy.transform.position, fairy.TrackSystem.Target.transform.position);
            if (dist > fairy.fairyCurrentStats.attackRange)
                fairy.ChangeState(new MoveState(fairy));
            else if (fairy.CanAttack)
                fairy.ChangeState(new AttackState(fairy));
        }
    }

    public override void Exit() { }
}
