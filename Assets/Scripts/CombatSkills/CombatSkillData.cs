using UnityEngine;

public abstract class CombatSkillData : ScriptableObject
{
    public enum SkillType
    {
        InvulShield,
        DmgBoost,
        DmgBoost2,
        DmgReduction
    }

    public SkillType skill;
    public int cost;
}