using UnityEngine;

public class StatsComponent : MonoBehaviour
{
    [SerializeField] private EnemySO enemySO;
    public float MaxHealth => enemySO.MaxHealth;
    public float AttackCooldown => enemySO.AttackCooldown;
    public float AttackDamage => enemySO.AttackDamage;
    public float AttackRange => enemySO.AttackRange;
    public float MoveSpeed => enemySO.MoveSpeed;
}
