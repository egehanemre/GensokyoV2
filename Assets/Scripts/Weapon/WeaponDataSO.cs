using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataSO", menuName = "Scriptable Objects/WeaponDataSO")]
public class WeaponDataSO : ScriptableObject
{
    public float attackDamage;
    public float attackCooldown;
    public float attackRange;

    public Mesh weaponMesh;

    public GameObject projectilePrefab;
    public float projectileSpeed;
    public float attackDuration;
}
