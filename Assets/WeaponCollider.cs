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
                float damage = ownerFairy.fairyCurrentStats.attackDamage;
                targetFairy.fairyCurrentStats.currentHealth -= damage;

                targetFairy.GetComponent<CombatManager>().DisableWeaponCollider();

                Vector3 knockbackDirection = (targetFairy.transform.position - ownerFairy.transform.position).normalized;

                targetFairy.ReactToHit(damage, knockbackDirection, ownerFairy.fairyCurrentStats.attackDamage * 2f);
            }
        }
    }
}
