using System;
using UnityEngine;

public enum BuffType
{
    DamageMultiplier,
    Invulnerable,
    DamageReduction,
}
public class Buff
{
    public BuffType Type;
    public float Value;
    public float Duration;
    public float TimeRemaining;
    public ParticleSystem VisualInstance;
    public CombatSkillData.SkillType? SourceSkillType; // Add this line

    public Buff(BuffType type, float value, float duration, CombatSkillData.SkillType? sourceSkillType = null)
    {
        Type = type;
        Value = value;
        Duration = duration;
        TimeRemaining = duration;
        VisualInstance = null;
        SourceSkillType = sourceSkillType;
    }
}