using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatSkillVisual : MonoBehaviour
{
    public TextMeshProUGUI skillCost;
    [SerializeField] private Image cooldownBarImage; // Assign in inspector
    [SerializeField] private Image skillIcon; // Assign in inspector
    [SerializeField] private Image skillBorder; 


    private float maxCooldown = 1f;

    public void SetMaxCooldown(float cooldown)
    {
        maxCooldown = Mathf.Max(1f, cooldown);
    }
    public void UpdateSkillVisual(int cost, float cooldown)
    {
        if (skillCost != null)
            skillCost.text = cost.ToString();

        if (cooldownBarImage != null)
        {
            float fill = Mathf.Clamp01(cooldown / maxCooldown);
            cooldownBarImage.fillAmount = fill;
        }

        if (skillIcon != null)
            skillIcon.color = (cooldown <= 0f) ? Color.white : new Color(1f, 1f, 1f, 0.3f);

        if (skillBorder != null)
            skillBorder.color = (cooldown <= 0f) ? Color.white : new Color(1f, 1f, 1f, 0.3f);
    }
}