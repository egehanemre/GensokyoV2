using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float damage;
    private Transform target;
    private float speed;

    private Team shooterTeam;

    public void Initialize(Transform target, float speed, float damage, Team team)
    {
        this.target = target;
        this.speed = speed;
        this.damage = damage;
        this.shooterTeam = team;
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void OnTriggerEnter(Collider other)
    {
        Fairy targetFairy = other.GetComponent<Fairy>();
        if (targetFairy != null)
        {
            if (targetFairy.Team == shooterTeam) return;

            Vector3 attackDirection = (targetFairy.transform.position - transform.position).normalized;
            Vector3 hitPoint = other.ClosestPoint(transform.position);

            targetFairy.ReactToHit(damage, Vector3.zero, 0, attackDirection, hitPoint, true);
            Destroy(gameObject);
        }
    }
}
