using System;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public class AbilityObjectStrategyFactory
    {
        public float LifeTime = 5;
        public virtual AbilityObjectStrategyRuntime CreateRuntime(AbilityContext ctx)
        {
            return new AbilityObjectStrategyRuntime(this, ctx);
        }
    }
}