using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Creates a targeting strategy that reads a selected target from external input.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Strategies/Targeting/Selected Target")]
    public class SelectedTargetingStrategyConfig : TargetingStrategyConfig
    {
        /// <summary>Creates a selected-target runtime for one ability activation.</summary>
        public override IAbilityStateStrategy CreateRuntime() => new SelectedTargetingRuntime(this);
    }
}
