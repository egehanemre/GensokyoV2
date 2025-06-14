using UnityEngine;
using TMPro;

public class RewardsManager : MonoBehaviour
{
    public static RewardsManager Instance { get; private set; }
    public GameObject rewardsPrefab;
    public Transform displayParent;

    private float _goldReward;
    public float goldReward
    {
        get => _goldReward;
        set => _goldReward = Mathf.Floor(value * 10f) / 10f;
    }

    private void Awake()
    {
        Instance = this;
    }

    public void DisplayRewards()
    {
        DisplayGoldReward();
    }

    private void DisplayGoldReward()
    {
        GameObject rewardGO = Instantiate(rewardsPrefab, displayParent);

        //not a good way to rely on component
        var goldText = rewardGO.GetComponentInChildren<TMP_Text>(true);
        if (goldText != null)
        {
            goldText.text = goldReward.ToString();
        }
    }
}
