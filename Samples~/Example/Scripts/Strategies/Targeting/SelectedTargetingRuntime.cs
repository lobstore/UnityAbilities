using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Uses the selected GameObject supplied through AbilityTargetingInput.
    /// Suitable for healing, curses, buffs, debuffs, and other object-targeted abilities.
    /// </summary>
    public class SelectedTargetingRuntime : TargetingStrategyRuntime
    {
        public SelectedTargetingRuntime(TargetingStrategyConfig config) : base(config) { }

        protected override void UpdateTargeting(AbilityContext ctx)
        {
            GameObject selectedTarget = ctx.TargetingInput != null
                ? ctx.TargetingInput.SelectedTarget
                : null;

            ctx.Target = selectedTarget;
            ctx.TargetPoint = selectedTarget != null
                ? selectedTarget.transform.position
                : ctx.Caster.transform.position;
        }
    }
}
