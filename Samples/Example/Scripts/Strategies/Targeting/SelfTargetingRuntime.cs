namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Sample Targeting strategy that selects the caster as both target and target point,
    /// then immediately skips the remaining targeting phase and enters Casting.
    /// </summary>
    public class SelfTargetingRuntime : TargetingStrategyRuntime
    {
        /// <summary>
        /// Creates a self-targeting runtime using the supplied base targeting configuration.
        /// </summary>
        /// <param name="cfg">Shared targeting settings for this ability.</param>
        public SelfTargetingRuntime(TargetingStrategyConfig cfg) : base(cfg) { }

        /// <summary>
        /// Initializes the base Targeting behavior, selects the caster, and transitions directly to Casting.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        public override void OnEnter(AbilityContext ctx)
        {
            base.OnEnter(ctx);
            ctx.Target = ctx.Caster;
            ctx.TargetPoint = ctx.Caster.transform.position;
            ctx.OwnerMachine.ChangeState(new CastingState(ctx.Ability.Data.Casting.CreateRuntime()));
        }

        /// <summary>
        /// Does not perform per-frame target updates because the caster is the fixed target.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        public override void OnUpdate(AbilityContext ctx) { }
    }
}
