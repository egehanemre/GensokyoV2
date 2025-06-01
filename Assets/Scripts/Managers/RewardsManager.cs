using UnityEngine;
using TMPro;

public class RewardsManager : MonoBehaviour
{
    public static RewardsManager Instance { get; private set; }
    public GameObject rewardsPrefab;
    public Transform displayParent;

    public float goldReward;

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
