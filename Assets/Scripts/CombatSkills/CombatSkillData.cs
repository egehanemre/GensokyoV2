using UnityEngine;

public abstract class CombatSkillData : ScriptableObject
{
    public enum SkillType
    {
        InvulShield,
        DmgBoost,
        DmgReduction
    }

    public SkillType skill;
    public int cost;
}