using System;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public class TargetingStrategyFactory : AbilityStateStrategyFactory
    {
        public override AbilityStrategyRuntime CreateRuntime(AbilityContext context) => new TargetingStrategyRuntime(this, context);
    }
}
