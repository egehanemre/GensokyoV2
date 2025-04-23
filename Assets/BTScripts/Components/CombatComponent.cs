using UnityEngine;

public class CombatComponent : MonoBehaviour
{
    private Animator animator;
    private StatsComponent stats;
    [SerializeField] private WeaponHitbox weaponHitbox;
    private float lastAttackTime;

    public Transform Target { get; set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        stats = GetComponent<StatsComponent>();
        weaponHitbox = GetComponent<WeaponHitbox>();
    }

    public void TryAttack()
    {
        if (Time.time - lastAttackTime < stats.AttackCooldown) return;
        if (Target == null || Vector3.Distance(transform.position, Target.position) > stats.AttackRange) return;

        animator.SetTrigger("Attack");
        weaponHitbox?.ActivateHitbox();

        lastAttackTime = Time.time;
        animator.SetFloat("moveSpeed", 0);
    }

    public void SetTarget(Transform target) => Target = target;
}
