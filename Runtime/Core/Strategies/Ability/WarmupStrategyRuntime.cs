namespace LOGIYGames.Abilities
{
    public class WarmupStrategyRuntime : AbilityStrategyRuntime
    {
        public WarmupStrategyRuntime(WarmupStrategyFactory cfg, AbilityContext context) : base(cfg, context) { }
        protected override AbilityPhase Phase => AbilityPhase.Warmup;
        protected override void StateTransitionRules()
            => _ctx.OwnerMachine.ChangeState(_ctx.CreateState(AbilityPhase.Targeting));
    }
}