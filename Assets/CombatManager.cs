using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private Fairy fairy;
    private TargetingManager targetingManager;
    private Animator animator;
    private Collider weaponCollider; 
    private void Awake()
    {
        fairy = GetComponent<Fairy>();
        targetingManager = GetComponent<TargetingManager>();
        animator = GetComponent<Animator>();

        weaponCollider = fairy.currentWeaponVisual.GetComponent<Collider>();
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false; 
        }
    }
    private void Update()
    {
        if (targetingManager == null || targetingManager.Target == null) return;
        HandleMovementAndCombat();
    }
    private void HandleMovementAndCombat()
    {
        if (fairy.IsStunned) return;

        GameObject targetObject = targetingManager.Target;
        if (targetObject == null) return;

        Fairy targetFairy = targetObject.GetComponent<Fairy>();
        if (targetFairy == null) return;

        float distanceToTarget = Vector3.Distance(fairy.transform.position, targetFairy.transform.position);

        // Prevent movement while attacking
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))  //very bad solution its temporary
        {
            animator.SetFloat("moveSpeed", 0);
            return;
        }

        if (distanceToTarget > fairy.fairyCurrentStats.attackRange)
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
        else
        {
            animator.SetFloat("moveSpeed", 0);
            TryAttack();
        }
    }

    private void RotateFairyTowardsTarget(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        fairy.transform.rotation = Quaternion.Slerp(fairy.transform.rotation, targetRotation, Time.deltaTime * 5f);
    }
    private void TryAttack()
    {
        if (Time.time - fairy.fairyCurrentStats.attackCooldown < 0) return;
        animator.SetTrigger("Attack");
        fairy.fairyCurrentStats.attackCooldown = Time.time + fairy.weaponDataSO.attackCooldown;
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
        //display attack range
        Gizmos.DrawWireSphere(transform.position, fairy.fairyCurrentStats.attackRange);
    }

}
