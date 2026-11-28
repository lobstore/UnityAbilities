using System;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public class ExitingStrategyFactory : AbilityStateStrategyFactory
    {
        public override AbilityStrategyRuntime CreateRuntime(AbilityContext ctx) => new ExitingStrategyRuntime(this, ctx);
    }
}
