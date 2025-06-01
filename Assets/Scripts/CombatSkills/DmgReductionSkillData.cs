using UnityEngine;

[CreateAssetMenu(menuName = "CombatSkill/DmgReductionSkillData")]
public class DmgReductionSkillData : CombatSkillData
{
    public float duration;
    [Range(0f, 1f)]
    public float reductionMultiplier; // e.g., 0.5 = 50% damage taken
}