using UnityEngine;
using System.Collections;

public class AttackState : FairyState
{
    private Fairy targetFairy;
    public AttackState(Fairy fairy) : base(fairy) { }
    public override void Enter()
    {
        targetFairy = fairy.TrackSystem.Target?.GetComponent<Fairy>();
        if (targetFairy == null)
        {
            fairy.ChangeState(new IdleState(fairy));
            return;
        }
    }

    public override void Update()
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
            return;
        }

        RotateTowardsTarget();

        if (fairy.CanAttack)
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        fairy.RegisterAttackCooldown();
        fairy.TriggerAnim("Attack");

        if (targetFairy != null)
        {
            Vector3 dir = (targetFairy.transform.position - fairy.transform.position).normalized;
            targetFairy.ReactToAttackStart(dir);
        }

        if (fairy.weaponDataSO.projectilePrefab == null)
        {
        }
        else
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
                fairy.Team
            );
        }

        fairy.StartMovementRoutine(StrafeAfterShooting());
    }

    private IEnumerator StrafeAfterShooting()
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 randDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-3f, 3f)).normalized;
        Vector3 targetPos = fairy.transform.position + randDir * Random.Range(0.5f, 2f);
        Rigidbody rb = fairy.Rigidbody;
        float speed = fairy.fairyCurrentStats.moveSpeed * 2f;

        fairy.Animator.SetFloat("moveSpeed", speed);

        while (Vector3.Distance(rb.position, targetPos) > 0.5f)
        {
            Vector3 dir = (targetPos - rb.position).normalized;
            Vector3 movePos = rb.position + dir * speed * Time.deltaTime;

            rb.MovePosition(movePos);
            RotateTowardsTarget();

            yield return null;
        }

        fairy.Animator.SetFloat("moveSpeed", 0f);
        fairy.StopMovementRoutine();
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
    }
    
}
