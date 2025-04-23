using UnityEngine;
using System.Collections;

public class HealthComponent : MonoBehaviour
{
    public float CurrentHealth { get; private set; }

    private StatsComponent stats;
    private Animator animator;
    private bool isStunned = false;

    private void Awake()
    {
        stats = GetComponent<StatsComponent>();
        animator = GetComponent<Animator>();
        CurrentHealth = stats.MaxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth -= damageAmount;
        animator.SetTrigger("Hit");

        StartCoroutine(HandleHitStun(0.5f)); // Customize via BT

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HandleHitStun(float duration)
    {
        isStunned = true;
        animator.SetFloat("moveSpeed", 0);
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    private void Die()
    {
        animator.SetTrigger("Death");
        Destroy(gameObject, 2f);
    }

    public bool IsDead => CurrentHealth <= 0;
    public bool IsStunned => isStunned;
}
