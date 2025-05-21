using UnityEngine;

[CreateAssetMenu(fileName = "FairyReactionConfig", menuName = "Fairy/Reaction Config")]
public class FairyReactionConfig : ScriptableObject
{
    [System.Serializable]
    public class ReactionEntry
    {
        public FairyBehavior behavior;
        public FairyType type;
        [Range(0, 100)] public float dodgeChance;
        [Range(0, 100)] public float blockChance;
    }

    public ReactionEntry[] reactions;

    public ReactionEntry GetReaction(FairyBehavior behavior, FairyType type)
    {
        foreach (var entry in reactions)
        {
            if (entry.behavior == behavior && entry.type == type)
                return entry;
        }
        return null;
    }
}
