namespace LOGIYGames.Abilities
{
    public class ReleaseStrategyRuntime : AbilityStrategyRuntime
    {
        ReleaseStrategyFactory _cfg;
        public ReleaseStrategyRuntime(ReleaseStrategyFactory cfg, AbilityContext ctx) : base(cfg, ctx) { _cfg = cfg; }

        protected override void StateTransitionRules()
        {
            if (_ctx.CurrentIteration - 1 < _cfg.Repeat)
            {
                switch (_cfg.RepeatFrom)
                {
                    case RepeatAbilityFrom.Targeting:
                        _ctx.OwnerMachine.ChangeState(new AbilityState(_ctx.Ability.Data.Targeting.CreateRuntime(_ctx)));
                        break;
                    case RepeatAbilityFrom.Casting:
                        _ctx.OwnerMachine.ChangeState(new AbilityState(_ctx.Ability.Data.Casting.CreateRuntime(_ctx)));
                        break;
                    case RepeatAbilityFrom.Release:
                        _ctx.OwnerMachine.ChangeState(new AbilityState(_ctx.Ability.Data.Release.CreateRuntime(_ctx)));
                        break;
                    default:
                        break;
                }
                return;
            }
            _ctx.OwnerMachine.ChangeState(new AbilityState(_ctx.Ability.Data.Exiting.CreateRuntime(_ctx)));
        }
        public override void OnEnter()
        {
            base.OnEnter();
            _ctx.CurrentIteration++;
            Release();
        }
        protected virtual void Release() { }

    }
}
