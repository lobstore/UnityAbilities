using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Strategies/Casting")]
    public class CastingStrategyConfig : AbilityStateStrategyConfig
    {
        public override IAbilityStateStrategy CreateRuntime() => new CastingStrategyRuntime(this);
    }
}
