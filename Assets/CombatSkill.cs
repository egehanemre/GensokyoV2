using TMPro;
using UnityEngine;

public class CombatSkill : MonoBehaviour
{
    public Skills skill;
    public int cost;

    // Duration and values can be set as needed
    public float shieldDuration = 8f;
    public float dmgBoostDuration = 8f;
    public float dmgMultiplier = 2f;

    public void ApplyCurrentSkillEffect(Fairy target)
    {
        if (target == null)
        {
            Debug.LogWarning("No target fairy to apply skill.");
            return;
        }

        switch (skill)
        {
            case Skills.Shield:
                // Apply invulnerability buff
                target.AddBuff(new Buff(BuffType.Invulnerable, 1f, shieldDuration));
                Debug.Log("Shield skill applied.");
                break;
            case Skills.DmgBoost:
                // Apply damage boost buff
                target.AddBuff(new Buff(BuffType.DamageMultiplier, dmgMultiplier, dmgBoostDuration));
                Debug.Log("Damage boost skill applied.");
                break;
            default:
                Debug.LogWarning("Unknown skill type.");
                break;
        }
    }

    public enum Skills
    {
        Shield,
        DmgBoost,
    }
}