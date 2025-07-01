using System.Linq;
using TMPro;
using UnityEngine;

public class DyingState : FairyState
{
    private float deathTime;
    private const float DESTROY_DELAY = 0.5f;
    public DyingState(Fairy fairy) : base(fairy) { }
    public override void Enter()
    {
        SoundFXManager.Instance.PlaySound(SoundFXManager.Instance.dieFX, fairy.transform, 0.5f);
        float goldEarned = 0f;
        if (fairy.Team == Team.Ally)
        {
            goldEarned = fairy.price * 0.35f;
            GoldManager.Instance?.AddGold(goldEarned);
            fairy.ShowGoldFeedback(goldEarned);

            FairyData deadData = PlayerUnits.Instance.OwnedFairies
            .FirstOrDefault(fd => fd.UniqueId == fairy.UniqueId);
            if (deadData != null)
            {
                DeadFairyDisplayManager.Instance?.ShowDeadFairy(deadData);
            }

            PlayerUnits.Instance.RemoveFairy(fairy.UniqueId);
        }
        else if (fairy.Team == Team.Enemy)
        {
            goldEarned = fairy.price * 0.75f;
            GoldManager.Instance?.AddGold(goldEarned);
            fairy.ShowGoldFeedback(goldEarned);
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