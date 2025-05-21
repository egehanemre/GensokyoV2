using UnityEngine;
using UnityEngine.UI;

public class AttackCooldownBar : MonoBehaviour
{
    [SerializeField] private Image cooldownBarSprite;
    [SerializeField] private float smoothSpeed = 5f;

    private float targetFill = 0f;
    private float currentFill = 0f;

    private void Awake()
    {
        if (cooldownBarSprite != null)
            currentFill = targetFill = cooldownBarSprite.fillAmount;
    }

    private void Update()
    {
        if (Camera.main != null)
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        if (cooldownBarSprite != null && !Mathf.Approximately(currentFill, targetFill))
        {
            currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * smoothSpeed);
            cooldownBarSprite.fillAmount = currentFill;
        }
    }

    public void UpdateCooldownBar(float cooldownFraction)
    {
        targetFill = Mathf.Clamp01(cooldownFraction);
    }
}
