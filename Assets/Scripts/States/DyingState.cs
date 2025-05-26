using System.Xml;
using UnityEngine;

public class DyingState : FairyState
{
    private float deathTime;
    private const float DESTROY_DELAY = 0.5f;
    public DyingState(Fairy fairy) : base(fairy) { }
    public override void Enter()
    {
        if (fairy.Team == Team.Ally)
        {
            GoldManager.Instance?.AddGold(fairy.price * 0.35f);
            PlayerUnits.Instance.RemoveFairy(fairy.UniqueId);
        }
        else if (fairy.Team == Team.Enemy)
        {
            GoldManager.Instance?.AddGold(fairy.price * 0.75f);
        }

        fairy.TriggerAnim("Die");

        if (fairy.Rigidbody != null)
            fairy.Rigidbody.isKinematic = true;

        if (fairy.currentWeaponVisual != null)
            fairy.currentWeaponVisual.SetActive(false);

        var trackSystem = fairy.GetComponent<TrackSystem>();
        if (trackSystem != null)
            trackSystem.OnUnitDeath();

        deathTime = Time.time + DESTROY_DELAY;
    }
    public override void Update()
    {
        if (Time.time >= deathTime)
        {
            Object.Destroy(fairy.gameObject);
        }
    }
    public override void Exit()
    {
    }
}
