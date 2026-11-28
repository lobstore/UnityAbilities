using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Strategies/Casting/Default")]
    public class CastingStrategyConfig : AbilityStateStrategyConfig
    {
        public override AbilityStrategyRuntime CreateRuntime(AbilityContext context) => new CastingStrategyRuntime(this, context);
    }
}
