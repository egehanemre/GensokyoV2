using UnityEngine;

public class AttackState : FairyState
{
    private Fairy targetFairy;
    private bool isAttacking = false;
    private float attackTimer = 0f;

    public AttackState(Fairy fairy) : base(fairy) { }

    public override void Enter()
    {
        targetFairy = fairy.TrackSystem.Target?.GetComponent<Fairy>();
        fairy.CurrentMoveSpeed = 0f;
        isAttacking = false;
    }

    public override void Update()
    {
        RotateTowardsTarget();

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
                // Check again after attack finished
                CheckTransitions();
            }
            return;
        }

        CheckTransitions();

        if (fairy.IsAttackReady())
        {
            PerformAttack();
        }
    }

    private void CheckTransitions()
    {
        if (targetFairy == null)
        {
            fairy.ChangeState(new IdleState(fairy));
            return;
        }

        float distance = Vector3.Distance(fairy.transform.position, targetFairy.transform.position);
        if (distance > fairy.fairyCurrentStats.attackRange)
        {
            fairy.ChangeState(new MoveState(fairy));
        }
    }

    private void PerformAttack()
    {
        isAttacking = true;
        attackTimer = fairy.weaponDataSO.attackDuration; 
        fairy.RegisterAttackCooldown();
        fairy.TriggerAnim("Attack");

        if (targetFairy != null && (fairy.TrackSystem.Match != null ))
        {
            Vector3 dir = (targetFairy.transform.position - fairy.transform.position).normalized;
            targetFairy.ReactToAttackStart(dir);
        }

        if (fairy.weaponDataSO.projectilePrefab != null)
        {
            ShootProjectile();
        }
    }

    private void ShootProjectile()
    {
        if (fairy.weaponDataSO.projectilePrefab == null || fairy.bowPosition == null) return;

        GameObject proj = GameObject.Instantiate(
            fairy.weaponDataSO.projectilePrefab,
            fairy.bowPosition.position,
            fairy.bowPosition.rotation
        );

        Projectile script = proj.GetComponent<Projectile>();
        if (script != null && targetFairy != null)
        {
            script.Initialize(
                targetFairy.transform,
                fairy.weaponDataSO.projectileSpeed,
                fairy.weaponDataSO.attackDamage,
                fairy.Team,
                fairy
            );
        }
    }

    private void RotateTowardsTarget()
    {
        if (targetFairy == null) return;
        Vector3 dir = (targetFairy.transform.position - fairy.transform.position).normalized;
        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        fairy.transform.rotation = Quaternion.Slerp(fairy.transform.rotation, targetRot, Time.deltaTime * 5f);
    }

    public override void Exit()
    {
        isAttacking = false;
    }
}
