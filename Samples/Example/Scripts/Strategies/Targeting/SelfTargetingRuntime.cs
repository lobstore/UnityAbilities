namespace LOGIYGames.Abilities
{
    public class SelfTargetingRuntime : TargetingStrategyRuntime
    {
        public SelfTargetingRuntime(TargetingStrategyConfig cfg) : base(cfg) { }

        public override void OnEnter(AbilityContext ctx)
        {
            base.OnEnter(ctx);
            ctx.Target = ctx.Caster;
            ctx.TargetPoint = ctx.Caster.transform.position;
            ctx.OwnerMachine.ChangeState(new CastingState(ctx.Ability.Data.Casting.CreateRuntime()));
        }
        public override void OnUpdate(AbilityContext ctx) { }
    }
}
