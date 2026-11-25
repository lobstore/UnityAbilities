using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Strategies/Warmup")]
    public class WarmupStrategyConfig : AbilityStateStrategyConfig
    {
        public override IAbilityStateStrategy CreateRuntime() => new WarmupStrategyRuntime(this);
    }
}
