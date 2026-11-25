using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Strategies/Release")]
    public class ReleaseStrategyConfig : AbilityStateStrategyConfig
    {
        [Min(1)]
        [Tooltip("Number of times this release phase is executed. Values greater than one return to Targeting between releases.")]
        public int MaxIterations = 1;

        public override IAbilityStateStrategy CreateRuntime()
        {
            return new ReleaseStrategyRuntime(this);
        }
    }
}
