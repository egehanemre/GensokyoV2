using UnityEngine;

public class LastChanceManager : MonoBehaviour
{
    [SerializeField] private GameObject lastChancePanel;
    [SerializeField] private float fadeDuration = 5f;

    private CanvasGroup canvasGroup;
    private bool wasActive = false;
    private Coroutine fadeCoroutine;
    private void Awake()
    {
        if (lastChancePanel != null)
        {
            canvasGroup = lastChancePanel.GetComponent<CanvasGroup>();
        }
    }
    private void Update()
    {
        bool shouldShow = CombatManager.consecutiveLosses == 1;

        if (shouldShow && !wasActive)
        {
            lastChancePanel.SetActive(true);
            canvasGroup.alpha = 1f;
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOut());
        }
        else if (!shouldShow && wasActive)
        {
            lastChancePanel.SetActive(false);
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
        }

        wasActive = shouldShow;
    }
    private System.Collections.IEnumerator FadeOut()
    {
        // Phase 1: 1f -> 0.9f over 4 seconds
        float phase1Duration = 2f;
        float phase2Duration = Mathf.Max(0f, fadeDuration - phase1Duration);

        float elapsed = 0f;
        while (elapsed < phase1Duration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0.9f, elapsed / phase1Duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0.9f;

        // Phase 2: 0.9f -> 0f over the rest
        elapsed = 0f;
        while (elapsed < phase2Duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0.9f, 0f, elapsed / phase2Duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
        lastChancePanel.SetActive(false);
    }
}