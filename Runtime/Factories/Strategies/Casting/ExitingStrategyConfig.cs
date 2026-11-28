using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Strategies/Exiting/Default")]
    public class ExitingStrategyConfig : AbilityStateStrategyConfig
    {
        public override AbilityStrategyRuntime CreateRuntime(AbilityContext ctx)
        {
            return new ExitingStrategyRuntime(this, ctx);
        }
    }
}
