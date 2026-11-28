namespace LOGIYGames.Abilities
{
    public class CastingStrategyRuntime : AbilityStrategyRuntime
    {
        public CastingStrategyRuntime(CastingStrategyConfig cfg, AbilityContext context) : base(cfg, context) {}

        protected override void StateTransitionRules()
        {
            _ctx.OwnerMachine.ChangeState(new AbilityState(_ctx.Ability.Data.Release.CreateRuntime(_ctx)));
        }
    }
}
