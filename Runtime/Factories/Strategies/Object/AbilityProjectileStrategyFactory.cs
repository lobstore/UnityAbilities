using System;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public class AbilityProjectileStrategyFactory : AbilityObjectStrategyFactory
    {
        public float Speed = 10;
        public override AbilityObjectStrategyRuntime CreateRuntime()
        {
            return new AbilityProjectileStrategyRuntime(this);
        }
    }
}