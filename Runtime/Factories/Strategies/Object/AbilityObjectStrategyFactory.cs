using System;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public class AbilityObjectStrategyFactory
    {
        public float LifeTime = 5;
        public virtual AbilityObjectStrategyRuntime CreateRuntime()
        {
            return new AbilityObjectStrategyRuntime(this);
        }
    }
}