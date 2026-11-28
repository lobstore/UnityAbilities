using System;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public class CastingStrategyFactory : AbilityStateStrategyFactory
    {
        public override AbilityStrategyRuntime CreateRuntime(AbilityContext ctx) => new CastingStrategyRuntime(this, ctx);
    }
}
