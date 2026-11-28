namespace LOGIYGames.Abilities
{
    public class TargetingStrategyRuntime : AbilityStrategyRuntime
    {
        public TargetingStrategyRuntime(TargetingStrategyFactory cfg, AbilityContext ctx) : base(cfg, ctx)
        {
            ctx.TargetingData = new();
        }

        protected override AbilityPhase Phase => AbilityPhase.Targeting;

        public override void OnUpdate()
        {
            UpdateTargeting();
            base.OnUpdate();
        }

        protected override void StateTransitionRules()
            => _ctx.OwnerMachine.ChangeState(_ctx.CreateState(AbilityPhase.Casting));

        protected virtual void UpdateTargeting()
        {
            if (_ctx.TargetingData != null && _ctx.CastOrigin != null)
                _ctx.TargetingData.Position = _ctx.CastOrigin.position;
        }
    }
}