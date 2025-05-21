using UnityEngine;

public class DodgeState : FairyState
{
    private Vector3 start;
    private Vector3 target;
    private float dodgeDuration;
    private float dodgeDistance;
    private float elapsed;

    public DodgeState(Fairy fairy, Vector3 dodgeDirection, float dodgeDuration, float dodgeDistance) : base(fairy)
    {
        this.dodgeDuration = dodgeDuration;
        this.dodgeDistance = dodgeDistance;
        dodgeDirection.y = 0;
        start = fairy.transform.position;
        target = start + dodgeDirection.normalized * dodgeDistance;
        elapsed = 0f;
    }

    public override void Enter()
    {
        fairy.TriggerAnim("Dodge");
        fairy.CurrentMoveSpeed = dodgeDistance / dodgeDuration;
    }

    public override void Update()
    {
        if (elapsed < dodgeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dodgeDuration);
            fairy.Rigidbody.MovePosition(Vector3.Lerp(start, target, t));
        }
        else
        {
            fairy.CurrentMoveSpeed = 0f;
            fairy.ChangeState(new IdleState(fairy));
        }
    }

    public override void Exit()
    {
        fairy.CurrentMoveSpeed = 0f;
    }
}
