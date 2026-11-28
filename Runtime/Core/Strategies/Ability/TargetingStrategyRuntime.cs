namespace LOGIYGames.Abilities
{
    public class TargetingStrategyRuntime : AbilityStrategyRuntime
    {
        public TargetingStrategyRuntime(TargetingStrategyFactory cfg, AbilityContext ctx) : base(cfg, ctx)
        {
            ctx.TargetingData = new();
        }

        public override void OnUpdate()
        {
            UpdateTargeting();
            base.OnUpdate();
        }
        protected override void StateTransitionRules()
        {
            _ctx.OwnerMachine.ChangeState(new AbilityState(_ctx.Ability.Data.Casting.CreateRuntime(_ctx)));
        }
        protected virtual void UpdateTargeting() { _ctx.TargetingData.Position = _ctx.CastOrigin.transform.position; }
    }
}
