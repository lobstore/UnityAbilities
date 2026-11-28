namespace LOGIYGames.Abilities
{
    public class ExitingStrategyRuntime : AbilityStrategyRuntime
    {
        public ExitingStrategyRuntime(AbilityStateStrategyFactory cfg, AbilityContext context) : base(cfg, context) {}

        protected override void StateTransitionRules()
        {
            _ctx.OwnerMachine.Finish();
        }
        public override void OnExit()
        {
            base.OnExit();
            _ctx.Ability.SetOnCooldown();
        }
    }
}
