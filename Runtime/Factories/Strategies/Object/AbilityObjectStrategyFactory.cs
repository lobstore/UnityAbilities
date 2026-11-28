using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public class AbilityObjectStrategyFactory
    {
        public float LifeTime = 5;
        [SerializeReference] public List<AbilityModuleFactory> Modules = new();
        public virtual AbilityObjectStrategyRuntime CreateRuntime(AbilityContext ctx)
            => new AbilityObjectStrategyRuntime(this, ctx);
    }
}