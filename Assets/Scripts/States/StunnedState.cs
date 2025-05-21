using UnityEngine;

public class StunnedState : FairyState
{
    private float stunEndTime;
    public StunnedState(Fairy fairy, float stunDuration) : base(fairy)
    {
        stunEndTime = Time.time + stunDuration;
    }

    public override void Enter()
    {
    }

    public override void Update()
    {
        if (Time.time >= stunEndTime)
        {
            fairy.ChangeState(new IdleState(fairy));
        }
    }

    public override void Exit() { }
}
