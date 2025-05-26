using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedManager : MonoBehaviour
{
    [SerializeField] private Button speedButton;
    [SerializeField] private TextMeshProUGUI speedButtonLabel;

    private readonly float[] speedSteps = { 1f, 2f, 4f};
    private int currentStep = 0;

    private void Start()
    {
        ResetSpeed();
        if (speedButton != null)
        {
            speedButton.onClick.AddListener(OnSpeedButtonClicked);
        }
        SetGameSpeed(speedSteps[currentStep]);
    }

    private void OnSpeedButtonClicked()
    {
        currentStep = (currentStep + 1) % speedSteps.Length;
        SetGameSpeed(speedSteps[currentStep]);
    }

    private void SetGameSpeed(float speed)
    {
        Time.timeScale = speed;
        if (speedButtonLabel != null)
        {
            speedButtonLabel.text = $"{speed}x";
        }
    }

    public void ResetSpeed()
    {
        currentStep = 0;
        SetGameSpeed(speedSteps[currentStep]);
    }
}