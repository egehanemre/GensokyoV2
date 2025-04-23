using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;
    private StatsComponent stats;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        stats = GetComponent<StatsComponent>();
    }

    public void Move(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;

        animator.SetFloat("moveSpeed", direction.magnitude);
        rb.MovePosition(rb.position + direction.normalized * stats.MoveSpeed * Time.fixedDeltaTime);
    }
}
