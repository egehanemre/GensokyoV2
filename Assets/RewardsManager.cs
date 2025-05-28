using UnityEngine;

public class RewardsManager : MonoBehaviour
{
    public static RewardsManager Instance { get; private set; }
    public GameObject rewardsPrefab;
    public Transform displayParent;

    private void Awake()
    {
        Instance = this;
    }
}
