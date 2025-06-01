using TMPro;
using UnityEngine;

public class CombatSkillVisual : MonoBehaviour
{
    public TextMeshProUGUI skillCost;
    public TextMeshProUGUI skillCooldown;

    public void UpdateSkillVisual(int cost, float cooldown)
    {
        skillCost.text = $"Cost: {cost}";
        if (cooldown > 0f)
            skillCooldown.text = $"CD: {cooldown:F1}s";
        else
            skillCooldown.text = "Ready";
    }
}