using TMPro;
using UnityEngine;

public class CombatSkillManager : MonoBehaviour
{
    private bool manaGainActive = false;
    public static CombatSkillManager Instance { get; private set; }

    [HideInInspector] public CombatSkill[] allSkills;
    [HideInInspector] public CombatSkillVisual[] allSkillVisuals;

    public Texture2D defaultCursor;
    public Texture2D shieldCursor;
    public Texture2D dmgBoostCursor;

    public int mana = 0;
    public int maxMana = 30;
    public float manaGainPerSec = 1f;
    private float manaFloat = 0f;
    public TextMeshProUGUI manaText;

    private float[] skillCooldowns;
    private Fairy selectedFairy;
    public CombatSkill currentSkill;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        allSkills = GetComponentsInChildren<CombatSkill>(includeInactive: true);
        allSkillVisuals = GetComponentsInChildren<CombatSkillVisual>(includeInactive: true);

        skillCooldowns = new float[allSkills.Length];
    }
    private void Update()
    {
        if (manaGainActive)
            UpdateMana();
        UpdateManaText();
        UpdateSkillCooldowns();
        UpdateSkillVisuals();
        HandleSkillModeInput();
    }
    public void SetManaGainActive(bool active)
    {
        manaGainActive = active;
    }

    private void UpdateMana()
    {
        if (mana < maxMana)
        {
            manaFloat += Time.deltaTime * manaGainPerSec;
            int manaToAdd = Mathf.FloorToInt(manaFloat);
            if (manaToAdd > 0)
            {
                mana = Mathf.Min(mana + manaToAdd, maxMana);
                manaFloat -= manaToAdd;
            }
        }
    }
    private void UpdateManaText()
    {
        if (manaText != null)
            manaText.text = $"Mana: {mana}/{maxMana}";
    }
    private void UpdateSkillCooldowns()
    {
        for (int i = 0; i < skillCooldowns.Length; i++)
        {
            if (skillCooldowns[i] > 0f)
                skillCooldowns[i] -= Time.deltaTime;
        }
    }
    private void UpdateSkillVisuals()
    {
        for (int i = 0; i < allSkills.Length; i++)
        {
            if (allSkillVisuals[i] != null && allSkills[i] != null && allSkills[i].skillData != null)
            {
                allSkillVisuals[i].UpdateSkillVisual(
                    allSkills[i].skillData.cost,
                    Mathf.Max(skillCooldowns[i], 0f)
                );
            }
        }
    }
    private void HandleSkillModeInput()
    {
        if (currentSkill != null && Input.GetMouseButtonDown(1))
            ExitSkillMode();

        if (currentSkill != null)
        {
            HandleTargetSelection();
            if (selectedFairy != null && Input.GetMouseButtonDown(0))
                TryUseSkill();
        }
    }
    private void TryUseSkill()
    {
        if (currentSkill == null || currentSkill.skillData == null) return;

        int skillIndex = System.Array.IndexOf(allSkills, currentSkill);
        if (skillIndex < 0) return;

        int requiredMana = currentSkill.skillData.cost;
        float requiredCooldown = GetSkillCooldown(currentSkill.skillData);

        if (mana >= requiredMana && skillCooldowns[skillIndex] <= 0f)
        {
            mana -= requiredMana;
            ApplySkillToSelectedFairy();
            skillCooldowns[skillIndex] = requiredCooldown;
            ExitSkillMode();
        }
        else
        {
            UpdateCursorForSkill(null);
        }
    }
    private float GetSkillCooldown(CombatSkillData skillData)
    {
        if (skillData == null) return 5f;

        switch (skillData.skill)
        {
            case CombatSkillData.SkillType.InvulShield:
                var shieldData = skillData as ShieldSkillData;
                return shieldData != null ? shieldData.duration : 5f;
            case CombatSkillData.SkillType.DmgBoost:
                var dmgData = skillData as DmgBoostSkillData;
                return dmgData != null ? dmgData.duration : 5f;
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
            hoveredFairy = hit.collider.GetComponentInParent<Fairy>();

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
            outline.enabled = selected;
    }
    public void SetCombatSkill(CombatSkill combatSkill)
    {
        int skillIndex = System.Array.IndexOf(allSkills, combatSkill);
        if (skillIndex < 0) return;
        if (skillCooldowns[skillIndex] > 0f) return;
        currentSkill = combatSkill;
        UpdateCursorForSkill(combatSkill.skillData);
    }
    private void UpdateCursorForSkill(CombatSkillData skillData)
    {
        if (skillData == null)
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            return;
        }

        switch (skillData.skill)
        {
            case CombatSkillData.SkillType.InvulShield:
                Cursor.SetCursor(shieldCursor, Vector2.zero, CursorMode.Auto);
                break;
            case CombatSkillData.SkillType.DmgBoost:
                Cursor.SetCursor(dmgBoostCursor, Vector2.zero, CursorMode.Auto);
                break;
            case CombatSkillData.SkillType.DmgReduction:
                Cursor.SetCursor(shieldCursor, Vector2.zero, CursorMode.Auto);
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
            currentSkill.ApplyCurrentSkillEffect(selectedFairy);
    }
}