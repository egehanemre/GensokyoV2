using UnityEngine;

public class FairyStats 
{
    public float maxHealth;
    public float currentHealth;
    public float moveSpeed;
    public float defense;

    public float attackDamage;
    public float attackCooldown;
    public float attackRange;

    public FairyStats(FairyStatsSO baseStats, WeaponData weapon)
    {
        maxHealth = baseStats.maxHealth;
        currentHealth = maxHealth;
        moveSpeed = baseStats.moveSpeed;
        defense = baseStats.defense;

        attackDamage = weapon.attackDamage;
        attackCooldown = weapon.attackCooldown;
        attackRange = weapon.attackRange;
    }
}
