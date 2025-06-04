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
            Vector3 nextPos = Vector3.Lerp(start, target, t);

            // Check for wall collision between current and next position
            Vector3 moveDir = (nextPos - fairy.Rigidbody.position).normalized;
            float moveDist = Vector3.Distance(fairy.Rigidbody.position, nextPos);

            // Use CapsuleCast to match the fairy's collider shape
            CapsuleCollider capsule = fairy.GetComponent<CapsuleCollider>();
            if (capsule != null)
            {
                Vector3 point1 = fairy.Rigidbody.position + capsule.center + Vector3.up * (capsule.height / 2 - capsule.radius);
                Vector3 point2 = fairy.Rigidbody.position + capsule.center - Vector3.up * (capsule.height / 2 - capsule.radius);

                if (Physics.CapsuleCast(point1, point2, capsule.radius, moveDir, out RaycastHit hit, moveDist, ~0, QueryTriggerInteraction.Ignore))
                {
                    // Hit a wall, stop dodge at the hit point
                    fairy.Rigidbody.MovePosition(hit.point);
                    fairy.CurrentMoveSpeed = 0f;
                    fairy.ChangeState(new IdleState(fairy));
                    return;
                }
            }
            else
            {
                // Fallback: use Raycast if no capsule
                if (Physics.Raycast(fairy.Rigidbody.position, moveDir, out RaycastHit hit, moveDist, ~0, QueryTriggerInteraction.Ignore))
                {
                    fairy.Rigidbody.MovePosition(hit.point);
                    fairy.CurrentMoveSpeed = 0f;
                    fairy.ChangeState(new IdleState(fairy));
                    return;
                }
            }

            fairy.Rigidbody.MovePosition(nextPos);
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
