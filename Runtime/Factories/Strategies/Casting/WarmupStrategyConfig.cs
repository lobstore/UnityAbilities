using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Strategies/Warmup/Default")]
    public class WarmupStrategyConfig : AbilityStateStrategyConfig
    {
        public override AbilityStrategyRuntime CreateRuntime(AbilityContext context) => new WarmupStrategyRuntime(this, context);
    }
}
