using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(TrackSystem))]
public class Fairy : MonoBehaviour
{
    private List<Buff> activeBuffs = new List<Buff>();

    public string UniqueId;
    public ParticleSystem damageParticles;
    public ParticleSystem blockParticles;
    public ParticleSystem projectileParticles;
    public ParticleSystem invulnerableBuffParticles;
    public ParticleSystem damageBoostBuffParticles;

    public float price = 0f;
    public GameObject fairyImageForShop;
    // Identifiers
    public Team Team;
    public FairyType fairyType;
    public FairyBehavior fairyBehavior;

    // Visuals & UI
    public Transform bowPosition;
    public TextMeshProUGUI fairyHP;
    public GameObject currentWeaponVisual;
    public MeshFilter weaponMeshFilter;
    public GameObject floatingTextPrefab;
    public GameObject floatingGoldPrefab;
    public GameObject healthBarPrefab;
    public GameObject attackCooldownBarPrefab;

    public float healthDisplay; 
    public AttackCooldownBar AttackCooldownBar { get; private set; }

    // Scriptable Object Data 
    public FairyStatsSO fairyStatsBase;
    public WeaponDataSO weaponDataSO;

    // Components & Runtime Data
    public HealthBar HealthBar { get; private set; }
    public Animator Animator { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public TrackSystem TrackSystem { get; private set; }
    public Collider WeaponCollider { get; private set; }
    public float CurrentMoveSpeed { get; set; }

    public FairyStats fairyCurrentStats;
    private WeaponData weaponData;
    private FairyState currentState;
    private float nextAttackTime = 0f;

    // Constants
    #region === Unity Methods ===
    private void Awake()
    {
        Outline outline = GetComponent<Outline>();
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        TrackSystem = GetComponent<TrackSystem>();
        WeaponCollider = currentWeaponVisual?.GetComponent<Collider>();

        InitializeStats();
        SetupHealthBar();
        ChangeState(new IdleState(this));
        DisableWeaponCollider();

        if(outline != null)
            outline.enabled = false;
    }
    public ParticleSystem PlayBuffVisual(BuffType buffType, Vector3 position)
    {
        ParticleSystem particles = null;
        switch (buffType)
        {
            case BuffType.Invulnerable:
                particles = invulnerableBuffParticles;
                break;
            case BuffType.DamageMultiplier:
                particles = damageBoostBuffParticles;
                break;
                // Add more cases for other buff types
        }

        if (particles != null)
        {
            ParticleSystem instance = Instantiate(particles, position, Quaternion.identity, this.transform);
            instance.transform.localPosition = this.transform.InverseTransformPoint(position);
            return instance;
        }
        return null;
    }

    public bool IsInvulnerable()
    {
        foreach (var buff in activeBuffs)
        {
            if (buff.Type == BuffType.Invulnerable)
                return true;
        }
        return false;
    }

    private void Update()
    {
        displayState = currentState?.ToString() ?? "None";
        currentState?.Update();

        healthDisplay = fairyCurrentStats.currentHealth;

        Animator.SetFloat("moveSpeed", CurrentMoveSpeed);
        UpdateAttackCooldownBar();
        UpdateBuffs(Time.deltaTime);
    }
    private void UpdateBuffs(float deltaTime)
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            activeBuffs[i].TimeRemaining -= deltaTime;
            if (activeBuffs[i].TimeRemaining <= 0)
            {
                if (activeBuffs[i].VisualInstance != null)
                {
                    Destroy(activeBuffs[i].VisualInstance.gameObject);
                }
                activeBuffs.RemoveAt(i);
            }
        }
    }
    public void AddBuff(Buff buff)
    {
        activeBuffs.Add(buff);
        buff.VisualInstance = PlayBuffVisual(buff.Type, transform.position);
    }

    public float GetDamageMultiplier()
    {
        float multiplier = 1f;
        foreach (var buff in activeBuffs)
        {
            if (buff.Type == BuffType.DamageMultiplier)
                multiplier *= buff.Value;
        }
        return multiplier;
    }
    #endregion

    #region === Initialization ===
    private void InitializeStats()
    {
        weaponData = new WeaponData(weaponDataSO);
        fairyCurrentStats = new FairyStats(fairyStatsBase, weaponData);
        SetWeaponMesh();
        RegisterAttackCooldown();
    }

    private void SetupHealthBar()
    {
        if (healthBarPrefab == null) return;

        GameObject bar = Instantiate(healthBarPrefab, transform);
        bar.transform.localPosition = new Vector3(0, 2f, 0);
        HealthBar = bar.GetComponent<HealthBar>();
        HealthBar?.UpdateHealthBar(fairyCurrentStats.maxHealth, fairyCurrentStats.currentHealth);

        AttackCooldownBar = bar.GetComponentInChildren<AttackCooldownBar>();
        AttackCooldownBar?.UpdateCooldownBar(0f);

        if(Team == Team.Ally)
        {
            HealthBar.healthBarSprite.color = new Color(0,0.3f,1);

        }
        else if (Team == Team.Enemy)
        {
            HealthBar.healthBarSprite.color = Color.red;
        }
    }

    private void UpdateAttackCooldownBar()
    {
        if (AttackCooldownBar != null)
        {
            float cooldown = weaponDataSO.attackCooldown;
            float timeLeft = Mathf.Clamp(nextAttackTime - Time.time, 0f, cooldown);
            float fraction = cooldown > 0f ? 1f - (timeLeft / cooldown) : 1f;
            AttackCooldownBar.UpdateCooldownBar(fraction);
        }
    }

    private void SetWeaponMesh()
    {
        if (weaponMeshFilter != null && weaponData.weaponMesh != null)
        {
            weaponMeshFilter.mesh = weaponData.weaponMesh;
        }
    }
    #endregion

    #region === State Management ===
    public void ChangeState(FairyState newState)
    {
        if (currentState is DyingState) return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
    #endregion

    #region === Attack Timing ===
    public bool IsAttackReady()
    {
        return Time.time >= nextAttackTime;
    }
    public void RegisterAttackCooldown()
    {
        nextAttackTime = Time.time + weaponDataSO.attackCooldown;
    }
    #endregion

    #region === Combat Reactions ===
    public void ReactToHit(float damage, Vector3 knockbackDirection, float knockbackForce, Vector3 attackDirection, Vector3 hitPoint, bool isProjectile = false)
    {
        if (IsInvulnerable())
        {
            ShowDamageFeedback("Invulnerable!");
            return;
        }

        if (currentState is BlockState blockState)
        {
            if (blockState.IsBlockingAttackFrom(attackDirection))
            {
                ShowDamageFeedback("Blocked!");
                Instantiate(blockParticles, hitPoint, Quaternion.identity);
                return;
            }
        }
        else
        {
            var particles = isProjectile ? projectileParticles : damageParticles;
            Instantiate(particles, hitPoint, Quaternion.identity);
        }

        ChangeState(new OnHitState(this, damage, knockbackDirection, knockbackForce));
    }

    public void ReactToAttackStart(Vector3 attackDirection)
    {
        ChangeState(new ReactionState(this, attackDirection));
    }
    #endregion

    #region === Animation & Weapon ===
    public void TriggerAnim(string triggerName)
    {
        Animator?.SetTrigger(triggerName);
    }

    public void EnableWeaponCollider()
    {
        WeaponCollider.enabled = true;

        WeaponCollider weaponScript = WeaponCollider.GetComponent<WeaponCollider>();
        if (weaponScript != null)
        {
            weaponScript.ResetHitFairies();
        }
    }
    public void DisableWeaponCollider()
    {
        if (WeaponCollider != null)
            WeaponCollider.enabled = false;
    }
    #endregion

    #region === Movement Helpers ===
    #endregion

    #region === UI Feedback ===
    public void ShowDamageFeedback(string text)
    {
        HealthBar?.UpdateHealthBar(fairyCurrentStats.maxHealth, fairyCurrentStats.currentHealth);
        ShowFloatingText(text);
    }
    public void ShowGoldFeedback(float gold)
    {
        ShowFloatingGold(gold);
    }

    private void ShowFloatingText(string text)
    {
        if (floatingTextPrefab == null) return;

        Vector3 offset = new Vector3(0, 2f, 0);
        GameObject go = Instantiate(floatingTextPrefab, transform.position + offset, Quaternion.identity);
        go.GetComponent<FloatingText>().text.text = text;
    }
    private void ShowFloatingGold(float gold)
    {
        if (floatingTextPrefab == null) return;

        Vector3 offset = new Vector3(0, 1f, 0);
        GameObject go = Instantiate(floatingGoldPrefab, transform.position + offset, Quaternion.identity);
        go.GetComponent<FloatingGold>().text.text = "+" + gold.ToString() + "c";
    }
    #endregion

    #region === Debug ===
    [SerializeField] public string displayState = "Idle";

    #endregion
}
