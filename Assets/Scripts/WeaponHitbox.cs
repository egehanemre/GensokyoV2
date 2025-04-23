using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f; 

    private bool isActive = false; 

    public void ActivateHitbox()
    {
        isActive = true;
    }

    // Deactivate the weapon hitbox (e.g., after the attack animation ends)
    public void DeactivateHitbox()
    {
        isActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return; 

        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            Vector3 hitDirection = (enemy.transform.position - transform.position).normalized;

            enemy.TakeDamage(damageAmount);

            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();

            Debug.Log($"{gameObject.name} hit {enemy.gameObject.name} for {damageAmount} damage.");
        }
    }
}

