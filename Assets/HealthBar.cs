using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarSprite;
    [SerializeField] private float smoothSpeed = 5f;

    private float targetFill = 1f;
    private float currentFill = 1f;

    private void Awake()
    {
        if (healthBarSprite != null)
            currentFill = targetFill = healthBarSprite.fillAmount;
    }

    private void Update()
    {
        if (Camera.main != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }

        if (healthBarSprite != null && !Mathf.Approximately(currentFill, targetFill))
        {
            currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * smoothSpeed);
            healthBarSprite.fillAmount = currentFill;
        }
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        targetFill = Mathf.Clamp01(currentHealth / maxHealth);
    }
}
