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

#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < reactions.Length; i++)
        {
            for (int j = i + 1; j < reactions.Length; j++)
            {
                if (reactions[i].behavior == reactions[j].behavior &&
                    reactions[i].type == reactions[j].type)
                {
                    Debug.LogWarning($"Duplicate reaction entry found: {reactions[i].behavior}/{reactions[i].type} at indices {i} and {j} in {name}", this);
                }
            }
        }
    }
#endif
}