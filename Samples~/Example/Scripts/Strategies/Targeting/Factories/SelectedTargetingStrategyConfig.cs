using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Creates a targeting strategy that reads a selected target from external input.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Strategies/Targeting/Selected Target")]
    public class SelectedTargetingStrategyConfig : TargetingStrategyConfig
    {
        public override IAbilityStateStrategy CreateRuntime() => new SelectedTargetingRuntime(this);
    }
}
