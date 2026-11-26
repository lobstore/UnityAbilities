using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Compatibility configuration for a cinematic Casting phase.
    /// Camera prefab settings are inherited from AbilityStateStrategyConfig.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Strategies/Casting/Cinemachine")]
    public class CinematicCastingStrategyConfig : CastingStrategyConfig
    {
        /// <summary>Creates a cinematic Casting runtime for one ability activation.</summary>
        public override IAbilityStateStrategy CreateRuntime()
            => new CinematicCastingStrategyRuntime(this);
    }
}