using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataSO", menuName = "Scriptable Objects/WeaponDataSO")]
public class WeaponDataSO : ScriptableObject
{
    public enum WeaponType
    {
        Melee,
        Ranged
    }

    public WeaponType weaponType;
    public float attackDamage;
    public float attackCooldown;
    public float attackRange;

    public Mesh weaponMesh;
}
