using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    private Fairy ownerFairy;

    private void Awake()
    {
        ownerFairy = GetComponentInParent<Fairy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fairy"))
        {
            Fairy targetFairy = other.GetComponent<Fairy>();
            if (targetFairy != null && targetFairy != ownerFairy)
            {
                CombatManager combatManager = targetFairy.GetComponent<CombatManager>();
                if (combatManager != null)
                {
                    combatManager.DisableWeaponCollider();
                }

                Vector3 attackDirection = (targetFairy.transform.position - ownerFairy.transform.position).normalized;

                float damage = ownerFairy.fairyCurrentStats.attackDamage;
                float knockbackForce = damage * 0.2f; // Optional: Replace with a configurable multiplier

                targetFairy.ReactToHit(damage, attackDirection, knockbackForce, attackDirection);
            }
        }
    }
}
