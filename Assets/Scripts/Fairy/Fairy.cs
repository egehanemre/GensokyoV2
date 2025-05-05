    using UnityEngine;
    using TMPro;
    using System.Collections;


    public class Fairy : MonoBehaviour
    {
        public FairyType fairyType; // For melee or ranged
        public Transform bowPosition; // For ranged weapons
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
}
