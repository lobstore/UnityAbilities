namespace LOGIYGames.Abilities
{
    public class CastingStrategyRuntime : AbilityStrategyRuntime
    {
        public CastingStrategyRuntime(CastingStrategyFactory cfg, AbilityContext context) : base(cfg, context) { }
        protected override AbilityPhase Phase => AbilityPhase.Casting;
        protected override void StateTransitionRules()
            => _ctx.OwnerMachine.ChangeState(_ctx.CreateState(AbilityPhase.Release));
    }
}