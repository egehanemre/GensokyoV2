using UnityEngine;
using TMPro;
using System.Collections;

public class Fairy : MonoBehaviour
{
    // Public Info
    public Team Team;
    public FairyType fairyType;
    public FairyBehavior fairyBehavior;

    // Visuals & UI
    public Transform bowPosition;
    public TextMeshProUGUI fairyHP;
    public GameObject currentWeaponVisual;
    public MeshFilter weaponMeshFilter;
    public GameObject floatingTextPrefab;
    public GameObject healthBarPrefab;
    public HealthBar healthBarVisual;

    // Scriptable Data
    public FairyStatsSO fairyStatsBase;
    public WeaponDataSO weaponDataSO;

    // Runtime Stats
    public FairyStats fairyCurrentStats;
    private WeaponData weaponData;

    // Components
    private Animator animator;
    private Rigidbody rb;

    // State
    private Coroutine movementRoutine = null;
    private bool isDead = false;
    private bool isBlocking = false;
    private bool isDodging = false;
    private float stunEndTime = 0f;

    // Constants
    private const float BLOCK_ANGLE = 45f;
    private const float STUN_DURATION = 1f;
    private const float BLOCK_DURATION = 2f;
    private const float DODGE_DISTANCE = 3f;
    private const float DODGE_DURATION = 0.2f;

    public bool IsMoving => movementRoutine != null;
    public bool IsStunned => Time.time < stunEndTime;
    public bool IsDodging => isDodging;
    public bool IsBlocking => isBlocking;

    #region Unity Methods

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        InitializeFairy();
        if (healthBarPrefab != null)
        {
            GameObject bar = Instantiate(healthBarPrefab, transform);
            // Adjust the local position to float above the fairy's head
            bar.transform.localPosition = new Vector3(0, 2f, 0); // Adjust Y as needed
            healthBarVisual = bar.GetComponent<HealthBar>();
        }
    }

    private void Update()
    {
        if (isDead) return;

        if (fairyCurrentStats.currentHealth <= 0)
        {
            Die();
            return;
        }
    }
    #endregion

    #region Initialization

    private void InitializeFairy()
    {
        weaponData = new WeaponData(weaponDataSO);
        fairyCurrentStats = new FairyStats(fairyStatsBase, weaponData);
        SetWeaponMesh();
    }

    private void SetWeaponMesh()
    {
        if (weaponMeshFilter != null && weaponData.weaponMesh != null)
        {
            weaponMeshFilter.mesh = weaponData.weaponMesh;
        }
    }

    #endregion

    #region Combat Reactions

    public void ReactToHit(float damage, Vector3 knockbackDirection, float knockbackForce, Vector3 attackDirection)
    {
        if (isDead) return;
        if (IsAttackBlocked(attackDirection))
        {
            UpdateHealthUI();
            return;
        }

        TriggerAnim("Hit");
        animator?.SetFloat("moveSpeed", 0);

        fairyCurrentStats.currentHealth -= damage;
        UpdateHealthUI(damage);

        ApplyKnockback(knockbackDirection, knockbackForce);
        stunEndTime = Time.time + STUN_DURATION;
    }

    public void ReactToAttackStart(Vector3 attackDirection)
    {
        if (isDead || isBlocking) return;

        float chance = Random.value * 100;

        switch (fairyBehavior)
        {
            case FairyBehavior.Evasive when fairyType != FairyType.Ranged:
                TryDodgeOrBlock(chance, attackDirection, 40f, 25f);
                break;

            case FairyBehavior.Turtle:
                TryDodgeOrBlock(chance, attackDirection, 20f, 45f);
                break;
        }
    }

    private void TryDodgeOrBlock(float chance, Vector3 attackDirection, float dodgeChance, float blockChance)
    {
        if (chance <= dodgeChance)
        {
            Dodge(attackDirection);
            return;
        }
        else if (chance <= blockChance)
        {
            Block(attackDirection);
        }
    }

    #endregion

    #region Blocking & Dodging

    private bool IsAttackBlocked(Vector3 attackDirection)
    {
        if (!isBlocking) return false;

        float angle = Vector3.Angle(transform.forward, -attackDirection.normalized);
        return angle <= BLOCK_ANGLE;
    }

    private void Block(Vector3 attackDirection)
    {
        StopMovementRoutine(); 
        attackDirection.y = 0;
        StartCoroutine(HoldBlock());
    }

    private IEnumerator HoldBlock()
    {
        isBlocking = true;
        TriggerAnim("Block");
        yield return new WaitForSeconds(BLOCK_DURATION);
        isBlocking = false;

    }

    private void Dodge(Vector3 attackDirection)
    {
        if (isBlocking) return;

        StopMovementRoutine();
        isDodging = true;

        Vector3 dodgeDirection = Quaternion.Euler(0, Random.Range(-90f, 90f), 0) * attackDirection.normalized;
        dodgeDirection.y = 0;

        TriggerAnim("Dodge");
        StartMovementRoutine(SmoothDodge(dodgeDirection, DODGE_DURATION, DODGE_DISTANCE));
    }

    private IEnumerator SmoothDodge(Vector3 direction, float duration, float distance)
    {
        float elapsed = 0f;
        Vector3 start = transform.position;
        Vector3 target = start + direction * distance;

        while (elapsed < duration)
        {
            rb.MovePosition(Vector3.Lerp(start, target, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(target);
        isDodging = false;
        movementRoutine = null;
    }
    #endregion

    #region Death & Knockback

    private void Die()
    {
        isDead = true;

        GetComponent<TrackSystem>()?.OnUnitDeath();
        TriggerAnim("Die");

        if (rb != null) rb.isKinematic = true;
        if (currentWeaponVisual != null) currentWeaponVisual.SetActive(false);

        Destroy(gameObject, 1f);
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (isBlocking || rb == null) return;

        StopMovementRoutine(); 
        direction.y = 0; 
        Vector3 knockbackTarget = transform.position + direction.normalized * force * 0.2f;
        StartMovementRoutine(SmoothKnockback(rb, knockbackTarget, 0.2f));
    }

    private IEnumerator SmoothKnockback(Rigidbody rb, Vector3 targetPos, float duration)
    {
        float elapsed = 0f;
        Vector3 start = rb.position;

        while (elapsed < duration)
        {
            Vector3 newPos = Vector3.Lerp(start, targetPos, elapsed / duration);
            rb.MovePosition(newPos);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetPos);
        movementRoutine = null;
    }

    #endregion

    #region Health UI

    private void UpdateHealthUI(float damage)
    {
        healthBarVisual.UpdateHealthBar(fairyCurrentStats.maxHealth, fairyCurrentStats.currentHealth);
        ShowFloatingText("-" + damage.ToString());
    }
    private void UpdateHealthUI()
    {
        ShowFloatingText("Blocked");
    }

    public void ShowFloatingText(string text)
    {
        Vector3 offset = new Vector3(0, 2f, 0);
        var go = Instantiate(floatingTextPrefab, transform.position + offset, Quaternion.identity);
        go.GetComponent<FloatingText>().text.text = text;
    }


    #endregion

    #region Movement Helpers

    public void StartMovementRoutine(IEnumerator routine)
    {
        if (isBlocking) return; 
        StopMovementRoutine();
        movementRoutine = StartCoroutine(routine);
    }

    public void StopMovementRoutine()
    {
        if (movementRoutine != null)
        {
            StopCoroutine(movementRoutine);
            movementRoutine = null;
        }
    }

    #endregion

    #region Debug

    #endregion

    #region Utility

    private void TriggerAnim(string triggerName)
    {
        if (animator != null)
            animator.SetTrigger(triggerName);
    }

    #endregion
}
