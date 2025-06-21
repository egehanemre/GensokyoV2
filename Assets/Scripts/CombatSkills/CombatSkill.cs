using UnityEngine;
using UnityEngine.UI;
public class CombatSkill : MonoBehaviour
{
    public CombatSkillData skillData;
    public Button skillButton;

    private void Awake()
    {
        skillButton = GetComponent<Button>();
        skillButton.onClick.AddListener(OnSkillButtonClicked);
    }

    private void OnDestroy()
    {
        if (skillButton != null)
        {
            skillButton.onClick.RemoveListener(OnSkillButtonClicked);
        }
    }
    private void OnSkillButtonClicked()
    {
        CombatSkillManager.Instance.SetCombatSkill(this);
    }
    public void ApplyCurrentSkillEffect(Fairy target)
    {
        if (target == null)
            return;

        switch (skillData.skill)
        {
            case CombatSkillData.SkillType.InvulShield:
                if (skillData is ShieldSkillData shieldData)
                    target.AddBuff(new Buff(BuffType.Invulnerable, 1f, shieldData.duration));
                break;
            case CombatSkillData.SkillType.DmgBoost:
                if (skillData is DmgBoostSkillData dmgData)
                {
                    target.AddBuff(new Buff(BuffType.DamageMultiplier, dmgData.multiplier, dmgData.duration));
                    // --- Visual effect: show damage boost on weapon ---
                    if (target.currentWeaponVisual != null && target.damageBoostBuffParticles != null)
                    {
                        ParticleSystem effect = Object.Instantiate(
                            target.damageBoostBuffParticles,
                            target.currentWeaponVisual.transform.position,
                            Quaternion.identity,
                            target.currentWeaponVisual.transform
                        );
                        effect.Play();
                    }
                }
                break;
            case CombatSkillData.SkillType.DmgBoost2:
                if (skillData is DmgBoostSkillData dmg2Data)
                {
                    target.AddBuff(new Buff(BuffType.DamageMultiplier, dmg2Data.multiplier, dmg2Data.duration));
                    // --- Visual effect: show damage boost 2 on weapon ---
                    if (target.currentWeaponVisual != null && target.damageBoostBuffParticles2 != null)
                    {
                        ParticleSystem effect = Object.Instantiate(
                            target.damageBoostBuffParticles2,
                            target.currentWeaponVisual.transform.position,
                            Quaternion.identity,
                            target.currentWeaponVisual.transform
                        );
                        effect.Play();
                    }
                }
                break;
            case CombatSkillData.SkillType.DmgReduction:
                if (skillData is DmgReductionSkillData reductionData)
                    target.AddBuff(new Buff(BuffType.DamageReduction, reductionData.reductionMultiplier, reductionData.duration));
                break;
            default:
                break;
        }
    }
}