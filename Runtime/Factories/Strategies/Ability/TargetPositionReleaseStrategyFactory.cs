using System;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public class TargetPositionReleaseStrategyFactory : ReleaseStrategyFactory
    {
        public bool ResetPitchRotation;
        public override AbilityStrategyRuntime CreateRuntime(AbilityContext ctx) => new TargetPositionReleaseStrategyRuntime(this, ctx);
    }
}
