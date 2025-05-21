using UnityEngine;

public class BlockState : FairyState
{
    private float blockDuration;
    private float blockEndTime;
    private Vector3 blockDirection; 

    public BlockState(Fairy fairy, float blockDuration, Vector3 blockDirection) : base(fairy)
    {
        this.blockDuration = blockDuration;
        this.blockDirection = blockDirection.normalized;
        this.blockDirection.y = 0;
    }

    public override void Enter()
    {
        fairy.TriggerAnim("Block");
        blockEndTime = Time.time + blockDuration;
    }

    public override void Update()
    {
        if (Time.time >= blockEndTime)
        {
            fairy.ChangeState(new IdleState(fairy));
        }
    }

    public override void Exit() { }

    public bool IsBlockingAttackFrom(Vector3 incomingDirection)
    {
        Vector3 attackDir = incomingDirection.normalized;
        attackDir.y = 0; 
        float angle = Vector3.Angle(blockDirection, attackDir);
        return angle <= 60f;
    }

    public Vector3 GetBlockDirection()
    {
        return blockDirection;
    }
}
