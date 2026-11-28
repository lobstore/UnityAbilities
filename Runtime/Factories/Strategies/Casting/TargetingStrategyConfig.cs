using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Strategies/Targeting/Default")]
    public class TargetingStrategyConfig : AbilityStateStrategyConfig
    {
        public override AbilityStrategyRuntime CreateRuntime(AbilityContext context) => new TargetingStrategyRuntime(this, context);
    }
}
