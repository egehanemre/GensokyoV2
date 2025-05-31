using UnityEngine;

public class CombatSkillManager : MonoBehaviour
{
    public CombatSkill currentSkill;

    public Texture2D defaultCursor;
    public Texture2D shieldCursor;
    public Texture2D dmgBoostCursor;

    private Fairy selectedFairy; // The currently selected target

    private void Update()
    {
        // Exit skill mode on right mouse button
        if (currentSkill != null && Input.GetMouseButtonDown(1))
        {
            ExitSkillMode();
        }

        // Only allow selection in skill mode
        if (currentSkill != null)
        {
            HandleTargetSelection();

            // Apply skill on left mouse button click
            if (selectedFairy != null && Input.GetMouseButtonDown(0))
            {
                ApplySkillToSelectedFairy();
                ExitSkillMode();
            }
        }
    }

    private void HandleTargetSelection()
    {
        // Raycast from mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Fairy hoveredFairy = null;

        if (Physics.Raycast(ray, out hit))
        {
            hoveredFairy = hit.collider.GetComponentInParent<Fairy>();
        }

        // Only select if it's a valid target (e.g., Team.Ally)
        if (hoveredFairy != null && hoveredFairy.Team == Team.Ally)
        {
            if (selectedFairy != hoveredFairy)
            {
                DeselectCurrentFairy();
                selectedFairy = hoveredFairy;
                // Optional: Add visual feedback, e.g., outline
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

    // Example: Visual feedback for selection (requires Outline or similar)
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

    // Apply the skill to the selected fairy
    public void ApplySkillToSelectedFairy()
    {
        if (currentSkill != null && selectedFairy != null)
        {
            // You may need to pass the fairy as a parameter if your skill system supports it
            // Example: currentSkill.ApplyCurrentSkillEffect(selectedFairy);
            currentSkill.ApplyCurrentSkillEffect(selectedFairy);
        }
    }
}