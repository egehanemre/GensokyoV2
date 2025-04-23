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

        if (other.TryGetComponent<HealthComponent>(out HealthComponent healthComponent))
        {
            healthComponent.TakeDamage(damageAmount);

            Debug.Log($"{gameObject.name} hit {other.gameObject.name} for {damageAmount} damage.");
        }
    }
}
