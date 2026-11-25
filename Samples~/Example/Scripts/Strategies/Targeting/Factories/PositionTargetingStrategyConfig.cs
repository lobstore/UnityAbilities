using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Creates a PositionTargetingRuntime that reads externally supplied world-space input.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Strategies/Targeting/Position")]
    public class PositionTargetingStrategyConfig : TargetingStrategyConfig
    {
        /// <summary>Creates a position-targeting runtime for one ability activation.</summary>
        public override IAbilityStateStrategy CreateRuntime() => new PositionTargetingRuntime(this);
    }
}
