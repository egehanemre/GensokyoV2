using UnityEngine;

public class ReactionState : FairyState
{
    private Vector3 attackDirection;
    public ReactionState(Fairy fairy, Vector3 attackDirection) : base(fairy)
    {
        this.attackDirection = attackDirection;
    }
    public override void Enter()
    {
        if ((fairy.fairyBehavior == FairyBehavior.Aggro || fairy.fairyType == FairyType.Ranged))
        {
            fairy.ChangeState(new IdleState(fairy));
            return;
        }

        var config = FairyReactionSettings.Config;
        var reaction = config?.GetReaction(fairy.fairyBehavior, fairy.fairyType);
        if (reaction != null)
        {
            TryDodgeOrBlock(reaction.dodgeChance, reaction.blockChance);
        }
        else
        {
            fairy.ChangeState(new IdleState(fairy));
        }
    }

    private void TryDodgeOrBlock(float dodgeChance, float blockChance)
    {
        float chance = Random.value * 100f;
        if (chance <= dodgeChance)
        {
            fairy.ChangeState(new DodgeState(fairy, GetDodgeDirection(), 0.2f, 3f));
        }
        else if (chance <= blockChance)
        {
            fairy.ChangeState(new BlockState(fairy, 1f, attackDirection));
        }
        else
        {
            fairy.ChangeState(new IdleState(fairy));
        }
    }
    private Vector3 GetDodgeDirection()
    {
        Vector3 dir = Quaternion.Euler(0, Random.Range(-90f, 90f), 0) * attackDirection.normalized;
        dir.y = 0;
        return dir;
    }
}