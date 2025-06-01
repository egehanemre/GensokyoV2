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
                    target.AddBuff(new Buff(BuffType.DamageMultiplier, dmgData.multiplier, dmgData.duration));
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