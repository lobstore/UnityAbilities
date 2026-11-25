using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Strategies/Release")]
    public class ReleaseStrategyConfig : AbilityStateStrategyConfig
    {
        /// <summary>
        /// Number of times the Release phase executes during one ability activation.
        /// Values greater than one return to Targeting between releases.
        /// </summary>
        [Min(1)]
        [Tooltip("Number of times this release phase is executed. Values greater than one return to Targeting between releases.")]
        public int MaxIterations = 1;

        /// <summary>Creates a fresh runtime strategy for one ability activation.</summary>
        /// <returns>A new release strategy.</returns>
        public override IAbilityStateStrategy CreateRuntime()
        {
            return new ReleaseStrategyRuntime(this);
        }
    }
}
