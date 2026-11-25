using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Configuration for the Release phase.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Strategies/Release")]
    public class ReleaseStrategyConfig : AbilityStateStrategyConfig
    {
        /// <summary>
        /// Creates a fresh Release runtime strategy using this configuration.
        /// Override this factory in derived configs to provide custom ability execution behavior.
        /// </summary>
        /// <returns>A new <see cref="ReleaseStrategyRuntime"/> instance.</returns>
        public override IAbilityStateStrategy CreateRuntime()
        {
            return new ReleaseStrategyRuntime(this);
        }
    }
}
