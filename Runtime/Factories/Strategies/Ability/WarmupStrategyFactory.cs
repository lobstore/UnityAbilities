using System;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public class WarmupStrategyFactory : AbilityStateStrategyFactory
    {
        public override AbilityStrategyRuntime CreateRuntime(AbilityContext context) => new WarmupStrategyRuntime(this, context);
    }
}
