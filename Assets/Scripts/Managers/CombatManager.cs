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
            MoveTowardsTarget(distanceToTarget);
        }
        else
        {
            animator.SetFloat("moveSpeed", 0);
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

        if (trackSystem.Match != null) // Check if the fairy has a match
        {
            Fairy matchedFairy = trackSystem.Match.GetComponent<Fairy>();
            if (matchedFairy != null)
            {
                // Notify the matched fairy to react to the attack
                Vector3 attackDirection = (matchedFairy.transform.position - fairy.transform.position).normalized;
                matchedFairy.ReactToAttackStart(attackDirection);
            }
        }

        if (fairy.weaponDataSO.projectilePrefab == null) // Melee attack
        {
            animator.SetTrigger("Attack");
            fairy.fairyCurrentStats.attackCooldown = Time.time + fairy.weaponDataSO.attackCooldown;
        }
        else // Ranged attack
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
            projectileScript.Initialize(targetFairy.transform, fairy.weaponDataSO.projectileSpeed, fairy.weaponDataSO.attackDamage);
        }
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
    private void OnDrawGizmosSelected() // for displaying attack range
    {
        if (fairy == null || fairy.fairyCurrentStats == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fairy.fairyCurrentStats.attackRange);
    }
}
