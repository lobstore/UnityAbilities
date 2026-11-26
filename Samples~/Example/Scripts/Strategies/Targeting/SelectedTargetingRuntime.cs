using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Targeting phase for abilities that require a GameObject selection, such as healing,
    /// curses, buffs, and debuffs. The player or AI supplies the selected object directly
    /// to AbilityContext.Target through AbilityController.CurrentContext.
    /// </summary>
    public class SelectedTargetingRuntime : TargetingStrategyRuntime
    {
        /// <summary>Creates a selected-target runtime using the shared targeting configuration.</summary>
        public SelectedTargetingRuntime(TargetingStrategyConfig config) : base(config) { }

        /// <summary>
        /// Leaves the selected target unchanged so input sources can update the context
        /// without the strategy overwriting their data.
        /// </summary>
        protected override void UpdateTargeting(AbilityContext ctx) { }
    }
}
