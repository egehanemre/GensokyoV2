using UnityEngine;

public class WeaponData
{
    public float attackDamage;
    public float attackCooldown;
    public float attackRange;
    public Mesh weaponMesh;

    public WeaponData(WeaponDataSO weaponData)
    {
        attackDamage = weaponData.attackDamage;
        attackCooldown = weaponData.attackCooldown;
        attackRange = weaponData.attackRange;
        weaponMesh = weaponData.weaponMesh;
    }
}
