using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Targeting strategy that consumes world-space input from AbilityContext.TargetingInput.
    /// It contains no Camera or UnityEngine.Input dependency, so the same strategy can be
    /// driven by a player input provider, AI, network input, or scripted gameplay.
    /// </summary>
    public class PositionTargetingRuntime : TargetingStrategyRuntime
    {
        /// <summary>Creates a position-targeting runtime using the shared targeting configuration.</summary>
        public PositionTargetingRuntime(TargetingStrategyConfig cfg) : base(cfg) { }

        /// <summary>Copies the externally supplied target and world position into the ability context.</summary>
        protected override void UpdateTargeting(AbilityContext ctx)
        {
            if (ctx.TargetingInput == null)
                return;

            ctx.Target = ctx.TargetingInput.SelectedTarget;

            if (ctx.TargetingInput.HasWorldPosition)
                ctx.TargetPoint = ctx.TargetingInput.WorldPosition;
            else if (ctx.Target != null)
                ctx.TargetPoint = ctx.Target.transform.position;
            else
                ctx.TargetPoint = Vector3.zero;
        }
    }
}
