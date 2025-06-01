using UnityEngine;

public static class FairyReactionSettings
{
    private static FairyReactionConfig config;

    public static FairyReactionConfig Config
    {
        get
        {
            if (config == null)
            {
                config = Resources.Load<FairyReactionConfig>("FairyReactionConfig");
            }
            return config;
        }
    }
}
