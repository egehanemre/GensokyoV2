using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatSkillVisual : MonoBehaviour
{
    public TextMeshProUGUI skillCost;
    [SerializeField] private Image cooldownBarImage; // Assign in inspector
    [SerializeField] private Image skillIcon; // Assign in inspector
    [SerializeField] private Image skillBorder;
    [SerializeField] private Image extraIcon; // Assign in inspector

    private float maxCooldown = 1f;
    public void SetMaxCooldown(float cooldown)
    {
        maxCooldown = Mathf.Max(1f, cooldown);
    }
    public void UpdateSkillVisual(int cost, float cooldown, bool hasEnoughMana)
    {
        if (skillCost != null)
            skillCost.text = cost.ToString();

        if (cooldownBarImage != null)
        {
            float fill = Mathf.Clamp01(cooldown / maxCooldown);
            cooldownBarImage.fillAmount = fill;
        }

        Color readyColor = Color.white;
        Color unavailableColor = new Color(0.3f, 0.3f, 0.3f, 1f); // Dark gray

        bool isUnavailable = cooldown > 0f || !hasEnoughMana;

        if (skillIcon != null)
            skillIcon.color = isUnavailable ? unavailableColor : readyColor;

        if (skillBorder != null)
            skillBorder.color = isUnavailable ? unavailableColor : readyColor;

        if (extraIcon != null)
            extraIcon.color = isUnavailable ? unavailableColor : readyColor;
    }
}
