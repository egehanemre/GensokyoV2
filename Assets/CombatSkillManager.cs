using TMPro;
using UnityEngine;

public class CombatSkillManager : MonoBehaviour
{
    public CombatSkill[] allSkills; // Assign all skills in the Inspector, order matches visuals
    public CombatSkillVisual[] allSkillVisuals; // Assign all visuals in the Inspector, order matches skills

    public Texture2D defaultCursor;
    public Texture2D shieldCursor;
    public Texture2D dmgBoostCursor;

    // Mana system
    public int mana = 0;
    public int maxMana = 30;
    public float manaGainPerSec = 1f;
    private float manaGainTimer = 0f;
    public TextMeshProUGUI manaText; // Assign in Inspector

    // Per-skill cooldowns
    private float[] skillCooldowns;

    private Fairy selectedFairy; // The currently selected target
    public CombatSkill currentSkill;

    private void Awake()
    {
        // Initialize cooldowns for all skills
        skillCooldowns = new float[allSkills.Length];
    }

    private void Update()
    {
        // Mana gain
        manaGainTimer += Time.deltaTime;
        if (manaGainTimer >= manaGainPerSec)
        {
            manaGainTimer -= manaGainPerSec;
            if (mana < maxMana)
                mana++;
        }

        if (manaText != null)
        {
            manaText.text = $"Mana: {mana}/{maxMana}";
        }

        // Update all skill cooldowns
        for (int i = 0; i < skillCooldowns.Length; i++)
        {
            if (skillCooldowns[i] > 0f)
                skillCooldowns[i] -= Time.deltaTime;
        }

        // --- Always update all skill visuals ---
        for (int i = 0; i < allSkills.Length; i++)
        {
            if (allSkillVisuals[i] != null && allSkills[i] != null)
            {
                allSkillVisuals[i].UpdateSkillVisual(
                    allSkills[i].cost,
                    Mathf.Max(skillCooldowns[i], 0f)
                );
            }
        }

        // Exit skill mode on right mouse button
        if (currentSkill != null && Input.GetMouseButtonDown(1))
        {
            ExitSkillMode();
        }

        // Only allow selection in skill mode
        if (currentSkill != null)
        {
            HandleTargetSelection();

            // Try to use skill on left mouse button click
            if (selectedFairy != null && Input.GetMouseButtonDown(0))
            {
                TryUseSkill();
            }
        }
    }

    private void TryUseSkill()
    {
        if (currentSkill == null) return;

        int skillIndex = System.Array.IndexOf(allSkills, currentSkill);
        if (skillIndex < 0) return;

        int requiredMana = currentSkill.cost;
        float requiredCooldown = GetSkillCooldown(currentSkill);

        if (mana >= requiredMana && skillCooldowns[skillIndex] <= 0f)
        {
            mana -= requiredMana;
            ApplySkillToSelectedFairy();
            skillCooldowns[skillIndex] = requiredCooldown;
            ExitSkillMode();
        }
        else
        {
            UpdateCursorForSkill(null); // Reset cursor if skill cannot be used
        }
    }

    private float GetSkillCooldown(CombatSkill skill)
    {
        // You can expand this for per-skill cooldowns
        switch (skill.skill)
        {
            case CombatSkill.Skills.Shield:
                return skill.shieldDuration;
            case CombatSkill.Skills.DmgBoost:
                return skill.dmgBoostDuration;
            default:
                return 5f;
        }
    }

    private void HandleTargetSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Fairy hoveredFairy = null;

        if (Physics.Raycast(ray, out hit))
        {
            hoveredFairy = hit.collider.GetComponentInParent<Fairy>();
        }

        if (hoveredFairy != null && hoveredFairy.Team == Team.Ally)
        {
            if (selectedFairy != hoveredFairy)
            {
                DeselectCurrentFairy();
                selectedFairy = hoveredFairy;
                SetFairySelected(selectedFairy, true);
            }
        }
        else
        {
            DeselectCurrentFairy();
        }
    }

    private void DeselectCurrentFairy()
    {
        if (selectedFairy != null)
        {
            SetFairySelected(selectedFairy, false);
            selectedFairy = null;
        }
    }

    private void SetFairySelected(Fairy fairy, bool selected)
    {
        var outline = fairy.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = selected;
        }
    }

    public void GetIntoSkillMode()
    {
        // Optionally, logic to enter skill mode
    }
    public void SetCombatSkill(CombatSkill combatSkill)
    {
        int skillIndex = System.Array.IndexOf(allSkills, combatSkill);
        if (skillIndex < 0) return;

        // If the skill is still in cooldown, do not allow selection
        if (skillCooldowns[skillIndex] > 0f)
            return;

        currentSkill = combatSkill;
        UpdateCursorForSkill(combatSkill);
    }
    private void UpdateCursorForSkill(CombatSkill skill)
    {
        if (skill == null)
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            return;
        }

        switch (skill.skill)
        {
            case CombatSkill.Skills.Shield:
                Cursor.SetCursor(shieldCursor, Vector2.zero, CursorMode.Auto);
                break;
            case CombatSkill.Skills.DmgBoost:
                Cursor.SetCursor(dmgBoostCursor, Vector2.zero, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
                break;
        }
    }

    public void ResetCursor()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    public void ExitSkillMode()
    {
        currentSkill = null;
        DeselectCurrentFairy();
        ResetCursor();
    }

    public void ApplySkillToSelectedFairy()
    {
        if (currentSkill != null && selectedFairy != null)
        {
            currentSkill.ApplyCurrentSkillEffect(selectedFairy);
        }
    }
}