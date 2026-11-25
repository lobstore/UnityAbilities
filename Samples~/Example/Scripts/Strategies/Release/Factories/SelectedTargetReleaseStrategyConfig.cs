using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Configuration for applying an ability effect to the selected Target object.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Strategies/Release/Selected Target")]
    public class SelectedTargetReleaseStrategyConfig : ReleaseStrategyConfig
    {
        /// <summary>Whether receiver components may be found on the target's parents as well as the target itself.</summary>
        public bool SearchParents = true;

        /// <summary>Creates a selected-target release runtime for one ability activation.</summary>
        public override IAbilityStateStrategy CreateRuntime()
            => new SelectedTargetReleaseStrategyRuntime(this);
    }
}
