using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private Fairy fairy;
    private TrackSystem trackSystem;
    private Animator animator;
    private Collider weaponCollider;
    private Fairy targetFairy;
    private void Awake()
    {
        fairy = GetComponent<Fairy>();
        trackSystem = GetComponent<TrackSystem>();
        animator = GetComponent<Animator>();
        weaponCollider = fairy.currentWeaponVisual.GetComponent<Collider>();

        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }
    private void Update()
    {
        HandleMovementAndCombat();

        if (trackSystem.Target != null)
        {
            Vector3 dir = (trackSystem.Target.transform.position - fairy.transform.position).normalized;
            RotateFairyTowardsTarget(dir);
        }
    }
    private void HandleMovementAndCombat()
    {
        if (fairy.IsStunned) return;

        if (trackSystem.Target != null)
            targetFairy = trackSystem.Target.GetComponent<Fairy>();

        if (targetFairy == null) return;

        float distance = Vector3.Distance(fairy.transform.position, targetFairy.transform.position);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            animator.SetFloat("moveSpeed", 0);
            return;
        }

        if (distance > fairy.fairyCurrentStats.attackRange)
        {
            if (fairy.fairyType == FairyType.Ranged && distance <= fairy.fairyCurrentStats.attackRange)
            {
                animator.SetFloat("moveSpeed", 0);
                return;
            }

            MoveTowardsTarget(distance);
        }
        else
        {
            if (fairy.fairyType != FairyType.Ranged)
                animator.SetFloat("moveSpeed", 0);

            TryAttack();
        }
    }
    private void MoveTowardsTarget(float distance)
    {
        Vector3 dir = (targetFairy.transform.position - fairy.transform.position).normalized;
        Rigidbody rb = fairy.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 newPos = rb.position + dir * fairy.fairyCurrentStats.moveSpeed * Time.deltaTime;
            rb.MovePosition(newPos);
        }

        RotateFairyTowardsTarget(dir);
        animator.SetFloat("moveSpeed", fairy.fairyCurrentStats.moveSpeed);
    }
    private void RotateFairyTowardsTarget(Vector3 dir)
    {
        if (dir.sqrMagnitude < 0.01f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        fairy.transform.rotation = Quaternion.Slerp(fairy.transform.rotation, targetRot, Time.deltaTime * 5f);
    }
    private void TryAttack()
    {
        if (Time.time < fairy.fairyCurrentStats.attackCooldown) return;

        if (trackSystem.Match != null)
        {
            var match = trackSystem.Match.GetComponent<Fairy>();
            if (match != null)
            {
                Vector3 dir = (match.transform.position - fairy.transform.position).normalized;
                match.ReactToAttackStart(dir);
                Debug.Log("Attack Start");
            }
        }
        if (fairy.weaponDataSO.projectilePrefab == null)
        {
            animator.SetTrigger("Attack");
        }
        else
        {
            ShootProjectile();
            animator.SetTrigger("Attack");
        }

        fairy.fairyCurrentStats.attackCooldown = Time.time + fairy.weaponDataSO.attackCooldown;
    }
    private void ShootProjectile()
    {
        if (fairy.weaponDataSO.projectilePrefab == null || fairy.bowPosition == null) return;

        GameObject proj = Instantiate(fairy.weaponDataSO.projectilePrefab, fairy.bowPosition.position, fairy.bowPosition.rotation);
        Projectile script = proj.GetComponent<Projectile>();

        if (script != null && targetFairy != null)
        {
            script.Initialize(targetFairy.transform, fairy.weaponDataSO.projectileSpeed, fairy.weaponDataSO.attackDamage, fairy.Team);
        }

        StartCoroutine(DelayedMoveAfterShoot());
    }
    private IEnumerator DelayedMoveAfterShoot()
    {
        yield return new WaitForSeconds(0.5f);
        MoveAfterShoot();
    }
    private void MoveAfterShoot()
    {
        if (fairy == null) return;

        Vector3 randDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-3f, 3f)).normalized;
        Vector3 newPos = fairy.transform.position + randDir * Random.Range(0.5f, 2f);

        fairy.StartMovementRoutine(SmoothStrafeMove(fairy.GetComponent<Rigidbody>(), newPos));
    }
    private IEnumerator SmoothStrafeMove(Rigidbody rb, Vector3 targetPos)
    {
        float speed = fairy.fairyCurrentStats.moveSpeed * 2f;

        animator.SetFloat("moveSpeed", speed);

        while (Vector3.Distance(rb.position, targetPos) > 0.5f)
        {
            Vector3 dir = (targetPos - rb.position).normalized;
            Vector3 movePos = rb.position + dir * speed * Time.deltaTime;

            rb.MovePosition(movePos);
            RotateFairyTowardsTarget(dir);

            yield return null;
        }

        animator.SetFloat("moveSpeed", 0f);
        fairy.StopMovementRoutine();
    }
    public void EnableWeaponCollider()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = true;
    }
    public void DisableWeaponCollider()
    {
        if (weaponCollider != null)
            weaponCollider.enabled = false;
    }
    private void OnDrawGizmosSelected()
    {
        if (fairy == null || fairy.fairyCurrentStats == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fairy.fairyCurrentStats.attackRange);
    }
}
