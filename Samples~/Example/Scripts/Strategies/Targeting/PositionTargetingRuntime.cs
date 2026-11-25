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
        public PositionTargetingRuntime(TargetingStrategyConfig cfg) : base(cfg) { }

        protected override void UpdateTargeting(AbilityContext ctx)
        {
            if (ctx.TargetingInput == null)
                return;

            ctx.Target = ctx.TargetingInput.SelectedTarget;

            if (ctx.TargetingInput.HasWorldPosition)
                ctx.TargetPoint = ctx.TargetingInput.WorldPosition;
            else if (ctx.Target != null)
                ctx.TargetPoint = ctx.Target.transform.position;
        }
    }
}
