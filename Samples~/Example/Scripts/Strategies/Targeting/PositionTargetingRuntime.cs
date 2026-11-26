using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Targeting phase for abilities whose world position is supplied by an external
    /// input source. The source writes directly to AbilityContext.Target and TargetPoint
    /// through AbilityController.CurrentContext; this strategy has no camera/input dependency.
    /// </summary>
    public class PositionTargetingRuntime : TargetingStrategyRuntime
    {
        /// <summary>Creates a position-targeting runtime using the shared targeting configuration.</summary>
        public PositionTargetingRuntime(TargetingStrategyConfig cfg) : base(cfg) { }

        /// <summary>
        /// Intentionally leaves Target and TargetPoint unchanged.
        /// They are populated directly on the active AbilityContext by the input source.
        /// </summary>
        protected override void UpdateTargeting(AbilityContext ctx) { }
    }
}
