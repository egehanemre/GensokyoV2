using UnityEngine;

public class OnHitState : FairyState
{
    private float damage;
    private Vector3 start;
    private Vector3 target;
    private float knockbackDuration = 0.2f;
    private float knockbackElapsed = 0f;
    private bool knockbackDone = false;
    private float knockbackForce;

    public OnHitState(Fairy fairy, float damage, Vector3 knockbackDirection, float knockbackForce) : base(fairy)
    {
        this.damage = damage;
        this.knockbackForce = knockbackForce;
        knockbackDirection.y = 0;
        start = fairy.transform.position;
        target = start + knockbackDirection.normalized * knockbackForce * 0.2f;
    }

    public override void Enter()
    {
        fairy.fairyCurrentStats.currentHealth -= damage;
        fairy.ShowDamageFeedback("-" + damage.ToString("F0"));

        if (fairy.fairyCurrentStats.currentHealth <= 0)
        {
            fairy.ChangeState(new DyingState(fairy));
            return;
        }

        fairy.TriggerAnim("Hit");
        fairy.CurrentMoveSpeed = 0f;
    }
    public override void Update()
    {
        if (fairy.fairyCurrentStats.currentHealth <= 0)
        {
            fairy.ChangeState(new DyingState(fairy));
            return;
        }

        if (!knockbackDone)
        {
            knockbackElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(knockbackElapsed / knockbackDuration);
            Vector3 nextPos = Vector3.Lerp(start, target, t);

            // Check for wall collision between current and next position
            Vector3 moveDir = (nextPos - fairy.Rigidbody.position).normalized;
            float moveDist = Vector3.Distance(fairy.Rigidbody.position, nextPos);

            CapsuleCollider capsule = fairy.GetComponent<CapsuleCollider>();
            if (capsule != null)
            {
                Vector3 center = fairy.Rigidbody.position + capsule.center;
                float halfHeight = Mathf.Max(0, (capsule.height * 0.5f) - capsule.radius);
                Vector3 point1 = center + Vector3.up * halfHeight;
                Vector3 point2 = center - Vector3.up * halfHeight;

                if (Physics.CapsuleCast(point1, point2, capsule.radius, moveDir, out RaycastHit hit, moveDist, ~0, QueryTriggerInteraction.Ignore))
                {
                    // Hit a wall, stop knockback at the hit point
                    fairy.Rigidbody.MovePosition(hit.point);
                    knockbackDone = true;
                    fairy.ChangeState(new StunnedState(fairy, 1f));
                    return;
                }
            }
            else
            {
                // Fallback: use Raycast if no capsule
                if (Physics.Raycast(fairy.Rigidbody.position, moveDir, out RaycastHit hit, moveDist, ~0, QueryTriggerInteraction.Ignore))
                {
                    fairy.Rigidbody.MovePosition(hit.point);
                    knockbackDone = true;
                    fairy.ChangeState(new StunnedState(fairy, 1f));
                    return;
                }
            }

            fairy.Rigidbody.MovePosition(nextPos);
            if (t >= 1f)
            {
                knockbackDone = true;
                fairy.ChangeState(new StunnedState(fairy, 1f));
            }
        }
    }
}
