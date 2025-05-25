using UnityEngine;
using System.Collections.Generic;

public class WeaponCollider : MonoBehaviour
{
    private Fairy ownerFairy;
    private HashSet<Fairy> hitFairies = new HashSet<Fairy>();

    private void Awake()
    {
        ownerFairy = GetComponentInParent<Fairy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fairy"))
        {
            Fairy targetFairy = other.GetComponent<Fairy>();

            if (targetFairy == null || targetFairy == ownerFairy || hitFairies.Contains(targetFairy))
                return;

            if (targetFairy.Team == ownerFairy.Team)
                return;

            hitFairies.Add(targetFairy);

            Vector3 attackDirection = (targetFairy.transform.position - ownerFairy.transform.position).normalized;
            float damage = ownerFairy.fairyCurrentStats.attackDamage;
            float knockbackForce = damage * 0.2f;

            targetFairy.ReactToHit(damage, attackDirection, knockbackForce, attackDirection);
        }
    }

    private void OnDrawGizmos()
    {
        if (ownerFairy == null)
            return;

        Gizmos.color = Color.red;
        Vector3 origin = transform.position;
        Vector3 direction = ownerFairy.transform.forward; 

        Gizmos.DrawLine(origin, origin + direction * 2f);
    }

    public void ResetHitFairies()
    {
        hitFairies.Clear();
    }
}
