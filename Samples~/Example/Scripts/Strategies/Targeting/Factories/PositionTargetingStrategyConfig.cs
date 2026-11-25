using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Creates a PositionTargetingRuntime that reads externally supplied world-space input.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Strategies/Targeting/Position")]
    public class PositionTargetingStrategyConfig : TargetingStrategyConfig
    {
        public override IAbilityStateStrategy CreateRuntime() => new PositionTargetingRuntime(this);
    }
}
