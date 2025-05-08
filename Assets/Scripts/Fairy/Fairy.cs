    using UnityEngine;
    using TMPro;
    using System.Collections;


    public class Fairy : MonoBehaviour
    {
        public FairyType fairyType; 
        public FairyBehavior fairyBehavior;
        public Transform bowPosition; 
        public TextMeshProUGUI fairyHP;
        public FairyStatsSO fairyStatsBase;
        public WeaponDataSO weaponDataSO;
        public FairyStats fairyCurrentStats;
        private WeaponData weaponData;
        public GameObject currentWeaponVisual;
        public MeshFilter weaponMeshFilter;
        private Animator animator;
        private Rigidbody rb;
        private bool isDead = false;
        private bool isBlocking = false; 
    private void Awake()
        {
            InitializeFairy();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
        }
        private void InitializeFairy()
        {
            weaponData = new WeaponData(weaponDataSO);
            fairyCurrentStats = new FairyStats(fairyStatsBase, weaponData);
            SetMesh();
        }
        private void Update()
        {
            if (isDead) return; 

            fairyHP.text = "HP: " + fairyCurrentStats.currentHealth.ToString("F0") + "/" + fairyCurrentStats.maxHealth.ToString("F0");

            if (fairyCurrentStats.currentHealth <= 0)
            {
                Die();
            }
        }
        private void SetMesh()
        {
            if (weaponMeshFilter != null && weaponData.weaponMesh != null)
            {
                weaponMeshFilter.mesh = weaponData.weaponMesh;
            }
        }
        public float stunEndTime = 0f;
        public bool IsStunned => Time.time < stunEndTime;
        public void ReactToHit(float damage, Vector3 knockbackDirection, float knockbackForce)
        {
            if (isDead) return; 

            animator?.SetTrigger("Hit");
            animator?.SetFloat("moveSpeed", 0);

            fairyCurrentStats.currentHealth -= damage;
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                knockbackDirection.y = 0; 
                rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            }
            stunEndTime = Time.time + 1f;
        }
    private void Die()
    {
        isDead = true;

        var trackSystem = GetComponent<TrackSystem>();
        if (trackSystem != null)
        {
            trackSystem.OnUnitDeath();
        }

        animator?.SetTrigger("Die");
        if (rb != null) rb.isKinematic = true;
        if (currentWeaponVisual != null) currentWeaponVisual.SetActive(false);
        Destroy(gameObject, 2f); 
    }
    public void ReactToAttackStart(Vector3 attackDirection)
    {
        if (isDead || isBlocking) return;

        float randomChance = Random.value * 100; // Random value between 0 and 100

        if (fairyBehavior == FairyBehavior.Evasive && (fairyType != FairyType.Ranged))
        {
            if (randomChance <= 25) // Dodge
            {
                Dodge(attackDirection);
                return;
            }
            //else if (randomChance <= 40) // Block
            //{
            //    Block(attackDirection);
            //    return;
            //}
        }
        else if (fairyBehavior == FairyBehavior.Turtle)
        {
            if (randomChance <= 20) // Dodge
            {
                Dodge(attackDirection);
                return;
            }
            //else if (randomChance <= 80) // Block
            //{
            //    Block(attackDirection);
            //    return;
            //}
        }
    }
    private void Dodge(Vector3 attackDirection)
    {
        Vector3 dodgeDirection = attackDirection.normalized;
        dodgeDirection.y = 0; 

        if (rb != null)
        {
            StartCoroutine(SmoothDodge(dodgeDirection, 0.2f, 3f)); 
        }

        animator?.SetTrigger("Dodge");
    }

    private IEnumerator SmoothDodge(Vector3 direction, float duration, float distance)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + direction * distance;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }
        transform.position = targetPosition;
    }
    //private void Block(Vector3 attackDirection)
    //{
    //    Quaternion blockRotation = Quaternion.LookRotation(attackDirection);
    //    transform.rotation = blockRotation;

    //    StartCoroutine(HoldBlock());
    //}

    //private IEnumerator HoldBlock()
    //{
    //    isBlocking = true;
    //    animator?.SetTrigger("Block");
    //    yield return new WaitForSeconds(2f); // Block duration
    //    isBlocking = false;
    //}
}
