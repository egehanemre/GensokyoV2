using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "Enemy/SO")]

public class EnemySO : ScriptableObject
{
    public float MaxHealth;
    public float AttackCooldown;
    public float AttackDamage;
    public float AttackRange;
    public float MoveSpeed;
}
