using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Authoring asset that defines an ability's identity, cooldown, and state strategy configurations.
    /// Create an asset through Assets > Create > Abilities > Ability Data.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Ability Data")]
    public class AbilityData : ScriptableObject
    {
        /// <summary>Display or lookup name of this ability.</summary>
        public string AbilityName;

        /// <summary>Cooldown duration in seconds, applied when the Release state exits.</summary>
        [Min(0)] public float CooldownDuration = 0;

        /// <summary>Configuration for the initial Warmup phase. The sample controller always starts the ability with this state, so assign a valid config.</summary>
        public WarmupStrategyConfig Warmup;

        /// <summary>Configuration for selecting or calculating the ability target.</summary>
        public TargetingStrategyConfig Targeting;

        /// <summary>Configuration for the cast-time phase before the ability is released.</summary>
        public CastingStrategyConfig Casting;

        /// <summary>Configuration for the phase that performs the ability's final gameplay action.</summary>
        public ReleaseStrategyConfig Release;
    }
}
