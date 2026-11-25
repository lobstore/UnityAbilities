using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Strategies/Targeting/Self")]
    public class SelfTargetingStrategyConfig : TargetingStrategyConfig
    {
        public override IAbilityStateStrategy CreateRuntime() => new SelfTargetingRuntime(this);
    }
}
