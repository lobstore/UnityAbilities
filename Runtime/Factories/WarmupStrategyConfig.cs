using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Configuration for the Warmup phase.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Strategies/Warmup")]
    public class WarmupStrategyConfig : AbilityStateStrategyConfig
    {
        /// <summary>
        /// Creates a fresh Warmup runtime strategy using this configuration.
        /// Override this factory in derived configs to provide custom preparation behavior.
        /// </summary>
        /// <returns>A new <see cref="WarmupStrategyRuntime"/> instance.</returns>
        public override IAbilityStateStrategy CreateRuntime() => new WarmupStrategyRuntime(this);
    }
}
