namespace LOGIYGames.Abilities
{
    public class ReleaseStrategyRuntime : AbilityStrategyRuntime
    {
        public ReleaseStrategyRuntime(ReleaseStrategyFactory cfg, AbilityContext context) : base(cfg, context) { }

        protected override void StateTransitionRules()
        {
            _ctx.OwnerMachine.ChangeState(new AbilityState(_ctx.Ability.Data.Exiting.CreateRuntime(_ctx)));
        }
        public override void OnEnter()
        {
            base.OnEnter();
            Release();
        }
        protected virtual void Release() { }
    }
}
