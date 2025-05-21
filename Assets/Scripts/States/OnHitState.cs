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
            fairy.Rigidbody.MovePosition(Vector3.Lerp(start, target, t));
            if (t >= 1f)
            {
                knockbackDone = true;
                fairy.ChangeState(new StunnedState(fairy, 1f));
            }
        }
    }
}
