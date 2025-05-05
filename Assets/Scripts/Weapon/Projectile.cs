using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float damage;
    private Transform target;
    private float speed;

    public void Initialize(Transform target, float speed, float damage)
    {
        this.target = target;
        this.speed = speed;
        this.damage = damage;
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
            targetFairy.ReactToHit(damage, Vector3.zero, 0); 
            Destroy(gameObject);
        }
    }
}
