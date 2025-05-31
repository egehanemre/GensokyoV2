using System;
using UnityEngine;

public enum BuffType
{
    DamageMultiplier,
    Invulnerable,
}

public class Buff
{
    public BuffType Type;
    public float Value;
    public float Duration;
    public float TimeRemaining;
    public ParticleSystem VisualInstance; // Add this

    public Buff(BuffType type, float value, float duration)
    {
        Type = type;
        Value = value;
        Duration = duration;
        TimeRemaining = duration;
        VisualInstance = null;
    }
}