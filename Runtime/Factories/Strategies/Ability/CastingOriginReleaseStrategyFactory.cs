using System;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public class CastingOriginReleaseStrategyFactory : ReleaseStrategyFactory
    {
        public bool ParentToCastingOrigin = false;
        public override AbilityStrategyRuntime CreateRuntime(AbilityContext ctx) => new CastingOriginReleaseStrategyRuntime(this, ctx);
    }
}
