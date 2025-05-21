using UnityEngine;

public class IdleState : FairyState
{
    public IdleState(Fairy fairy) : base(fairy) { }
    public override void Enter()
    {
        fairy.TriggerAnim("Idle");
    }
    public override void Update()
    {
        var target = fairy.TrackSystem.Target;
        if (target == null)
            return;

        float dist = Vector3.Distance(fairy.transform.position, target.transform.position);
        if (dist > fairy.fairyCurrentStats.attackRange)
        {
            fairy.ChangeState(new MoveState(fairy));
        }
        else
        {
            fairy.ChangeState(new AttackState(fairy));
        }
    }
    public override void Exit() { }
}
