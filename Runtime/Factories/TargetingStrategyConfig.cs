using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Base configuration for Targeting strategies.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Strategies/Targeting/Basic")]
    public class TargetingStrategyConfig : AbilityStateStrategyConfig
    {
        /// <summary>Maximum time in seconds spent selecting or updating the target before transitioning to Casting.</summary>
        public float MaxTargetingTime = 2f;

        /// <summary>
        /// Creates a fresh default Targeting runtime strategy.
        /// Override this factory in derived configs to create custom targeting behavior.
        /// </summary>
        /// <returns>A new <see cref="TargetingStrategyRuntime"/> instance.</returns>
        public override IAbilityStateStrategy CreateRuntime() => new TargetingStrategyRuntime(this);
    }
}
