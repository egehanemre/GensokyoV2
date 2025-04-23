//using System.Collections;
//using UnityEngine;

//public abstract class Enemy : MonoBehaviour
//{
//    public Animator animator;
//    public WeaponHitbox weaponHitbox;
//    public Rigidbody rb { get; private set; }
//    public Transform Target { get; set; }

//    [SerializeField] private EnemySO enemySO;
//    [SerializeField] private bool isAlly; // Ally vs Enemy toggle
//    public float MaxHealth => enemySO.MaxHealth;
//    public float CurrentHealth { get; set; }
//    public float AttackCooldown => enemySO.AttackCooldown;
//    public float AttackDamage => enemySO.AttackDamage;
//    public float AttackRange => enemySO.AttackRange;
//    public float MoveSpeed => enemySO.MoveSpeed;

//    private float lastAttackTime;
//    private bool isStunned = false; // Track if the enemy is stunned

//    protected virtual void Awake()
//    {
//        rb = GetComponent<Rigidbody>();
//        animator = GetComponent<Animator>();
//        CurrentHealth = MaxHealth;
//        weaponHitbox = GetComponentInChildren<WeaponHitbox>();
//    }

//    protected virtual void Start()
//    {
//        if (isAlly)
//            CombatManager.Instance.RegisterPlayerSideEnemy(this);
//        else
//            CombatManager.Instance.RegisterEnemySideEnemy(this);
//    }

//    private void OnDestroy()
//    {
//        CombatManager.Instance.UnregisterEnemy(this);
//    }

//    private void FixedUpdate()
//    {
//        if (isStunned || Target == null || CurrentHealth <= 0) return; // Prevent movement if stunned

//        float distanceToTarget = Vector3.Distance(transform.position, Target.position);

//        if (distanceToTarget <= AttackRange)
//        {
//            Attack();
//        }
//        else
//        {
//            Vector3 direction = (Target.position - transform.position).normalized;
//            Move(direction);
//        }
//    }

//    public void Move(Vector3 direction)
//    {
//        animator.SetFloat("moveSpeed", direction.magnitude);
//        rb.MovePosition(rb.position + direction * MoveSpeed * Time.fixedDeltaTime);
//    }

//    public void Attack()
//    {
//        if (Time.time - lastAttackTime < AttackCooldown) return;

//        if (Target != null && Vector3.Distance(transform.position, Target.position) <= AttackRange)
//        {
//            animator.SetTrigger("Attack");

//            // Activate the weapon hitbox
//            if (weaponHitbox != null)
//            {
//                weaponHitbox.ActivateHitbox();
//            }

//            lastAttackTime = Time.time;
//            animator.SetFloat("moveSpeed", 0);
//        }
//    }

//    public void TakeDamage(float damageAmount)
//    {
//        if (CurrentHealth <= 0) return;

//        CurrentHealth -= damageAmount;
//        Debug.Log($"{gameObject.name} took {damageAmount} damage. Current health: {CurrentHealth}");

//        animator.SetTrigger("Hit");
//        StartCoroutine(HandleHitStun(0.5f)); // Example: 0.5 seconds stun duration

//        if (CurrentHealth <= 0)
//        {
//            Die();
//        }
//    }

//    private IEnumerator HandleHitStun(float duration)
//    {
//        isStunned = true;
//        animator.SetFloat("moveSpeed", 0); // Stop movement during stun
//        Debug.Log($"{gameObject.name} is stunned for {duration} seconds.");

//        yield return new WaitForSeconds(duration);

//        isStunned = false;
//        Debug.Log($"{gameObject.name} is no longer stunned.");
//    }

//    public void ActivateWeaponHitbox()
//    {
//        if (weaponHitbox != null)
//        {
//            weaponHitbox.ActivateHitbox();
//        }
//    }
//    public void DeactivateWeaponHitbox()
//    {
//        if (weaponHitbox != null)
//        {
//            weaponHitbox.DeactivateHitbox();
//        }
//    }
//    public void Die()
//    {
//        animator.SetTrigger("Death");
//        Destroy(gameObject, 2f);
//    }
//}
