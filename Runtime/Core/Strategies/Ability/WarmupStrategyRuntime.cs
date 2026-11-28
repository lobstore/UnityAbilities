namespace LOGIYGames.Abilities
{
    public class WarmupStrategyRuntime : AbilityStrategyRuntime
    {
        public WarmupStrategyRuntime(WarmupStrategyFactory cfg, AbilityContext context) : base(cfg, context) {}

        protected override void StateTransitionRules()
        {
            _ctx.OwnerMachine.ChangeState(new AbilityState(_ctx.Ability.Data.Targeting.CreateRuntime(_ctx)));
        }
    }
}
