using System;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Legacy compatibility wrapper. New assets should use PositionTargetingRuntime,
    /// which reads camera-independent data from AbilityContext.TargetingInput.
    /// </summary>
    [Obsolete("Use PositionTargetingRuntime with PositionTargetingStrategyConfig.")]
    public class DirectionalTargetingRuntime : PositionTargetingRuntime
    {
        public DirectionalTargetingRuntime(TargetingStrategyConfig config) : base(config) { }
    }
}
