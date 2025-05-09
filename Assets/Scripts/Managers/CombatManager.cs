using UnityEngine;
using TMPro;
using System.Collections;

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
        {
            weaponCollider.enabled = false;
        }
    }

    private void Update()
    {
        HandleMovementAndCombat();

        if (trackSystem.Target != null)
        {
            Vector3 direction = (trackSystem.Target.transform.position - fairy.transform.position).normalized;
            RotateFairyTowardsTarget(direction);
        }
    }

    private void HandleMovementAndCombat()
    {
        if (fairy.IsStunned) return;

        if (trackSystem.Target != null)
        {
            targetFairy = trackSystem.Target.GetComponent<Fairy>();
        }

        if (targetFairy == null) return;

        float distanceToTarget = Vector3.Distance(fairy.transform.position, targetFairy.transform.position);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            animator.SetFloat("moveSpeed", 0);
            return;
        }

        if (distanceToTarget > fairy.fairyCurrentStats.attackRange)
        {
            if (fairy.fairyType == FairyType.Ranged && distanceToTarget <= fairy.fairyCurrentStats.attackRange)
            {
                animator.SetFloat("moveSpeed", 0);
                return;
            }

            MoveTowardsTarget(distanceToTarget);
        }
        else
        {
            if(fairy.fairyType != FairyType.Ranged)
            {
                animator.SetFloat("moveSpeed", 0);
            }
            TryAttack();
        }
    }


    private void MoveTowardsTarget(float distanceToTarget)
    {
        Vector3 direction = (targetFairy.transform.position - fairy.transform.position).normalized;

        Rigidbody rb = fairy.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 newPosition = rb.position + direction * fairy.fairyCurrentStats.moveSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);
        }

        RotateFairyTowardsTarget(direction);
        animator.SetFloat("moveSpeed", fairy.fairyCurrentStats.moveSpeed);
    }

    private void RotateFairyTowardsTarget(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        fairy.transform.rotation = Quaternion.Slerp(fairy.transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    private void TryAttack()
    {
        if (Time.time < fairy.fairyCurrentStats.attackCooldown) return;

        if (trackSystem.Match != null)
        {
            Fairy matchedFairy = trackSystem.Match.GetComponent<Fairy>();
            if (matchedFairy != null)
            {
                Vector3 attackDirection = (matchedFairy.transform.position - fairy.transform.position).normalized;
                matchedFairy.ReactToAttackStart(attackDirection);
            }
        }

        if (fairy.weaponDataSO.projectilePrefab == null)
        {
            animator.SetTrigger("Attack");
            fairy.fairyCurrentStats.attackCooldown = Time.time + fairy.weaponDataSO.attackCooldown;
        }
        else
        {
            ShootProjectile();
            animator.SetTrigger("Attack");
            fairy.fairyCurrentStats.attackCooldown = Time.time + fairy.weaponDataSO.attackCooldown;
        }
    }

    private void ShootProjectile()
    {
        if (fairy.weaponDataSO.projectilePrefab == null || fairy.bowPosition == null) return;

        GameObject projectile = Instantiate(
            fairy.weaponDataSO.projectilePrefab,
            fairy.bowPosition.position,
            fairy.bowPosition.rotation
        );

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null && targetFairy != null)
        {
            projectileScript.Initialize(targetFairy.transform, fairy.weaponDataSO.projectileSpeed, fairy.weaponDataSO.attackDamage, fairy.Team);
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

        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-3f, 3f)).normalized;

        float moveDistance = Random.Range(0.5f, 2f);
        Vector3 targetPosition = fairy.transform.position + randomDirection * moveDistance;

        fairy.StartMovementRoutine(SmoothStrafeMove(fairy.GetComponent<Rigidbody>(), targetPosition));
    }
    private IEnumerator SmoothStrafeMove(Rigidbody rb, Vector3 targetPosition)
    {
        float moveSpeed = fairy.fairyCurrentStats.moveSpeed * 2f;
        float minDistance = 0.5f;

        animator.SetFloat("moveSpeed", moveSpeed);

        while (Vector3.Distance(rb.position, targetPosition) > minDistance)
        {
            Vector3 direction = (targetPosition - rb.position).normalized;
            Vector3 newPosition = rb.position + direction * moveSpeed * Time.deltaTime;

            rb.MovePosition(newPosition);

            RotateFairyTowardsTarget(direction);

            yield return null;
        }

        animator.SetFloat("moveSpeed", 0f);
        fairy.StopMovementRoutine();
    }


    public void EnableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
    }
    public void DisableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (fairy == null || fairy.fairyCurrentStats == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fairy.fairyCurrentStats.attackRange);
    }
}
