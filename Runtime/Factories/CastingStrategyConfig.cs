using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Configuration for the Casting phase.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Strategies/Casting")]
    public class CastingStrategyConfig : AbilityStateStrategyConfig
    {
        /// <summary>
        /// Creates a fresh Casting runtime strategy using this configuration.
        /// Override this factory in derived configs to provide custom cast-time behavior.
        /// </summary>
        /// <returns>A new <see cref="CastingStrategyRuntime"/> instance.</returns>
        public override IAbilityStateStrategy CreateRuntime() => new CastingStrategyRuntime(this);
    }
}
