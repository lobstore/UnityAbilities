namespace LOGIYGames.Abilities
{
    public class ReleaseStrategyRuntime : AbilityStrategyRuntime
    {
        private readonly ReleaseStrategyFactory _cfg;
        public ReleaseStrategyRuntime(ReleaseStrategyFactory cfg, AbilityContext ctx) : base(cfg, ctx) { _cfg = cfg; }
        protected override AbilityPhase Phase => AbilityPhase.Release;

        protected override void StateTransitionRules()
        {
            if (_ctx.Services != null && _ctx.Services.Flow != null && _ctx.Services.Flow.HasModules)
            {
                if (_ctx.Services.Flow.TryResolveNextPhase(out AbilityPhase nextPhase))
                    _ctx.OwnerMachine.ChangeState(_ctx.CreateState(nextPhase));
                else
                    _ctx.OwnerMachine.ChangeState(_ctx.CreateState(AbilityPhase.Exiting));
                return;
            }

            // Existing assets remain compatible with the original Repeat fields.
            if (_ctx.CurrentIteration - 1 < _cfg.Repeat)
            {
                switch (_cfg.RepeatFrom)
                {
                    case RepeatAbilityFrom.Targeting:
                        _ctx.OwnerMachine.ChangeState(_ctx.CreateState(AbilityPhase.Targeting)); break;
                    case RepeatAbilityFrom.Casting:
                        _ctx.OwnerMachine.ChangeState(_ctx.CreateState(AbilityPhase.Casting)); break;
                    case RepeatAbilityFrom.Release:
                        _ctx.OwnerMachine.ChangeState(_ctx.CreateState(AbilityPhase.Release)); break;
                    default:
                        _ctx.OwnerMachine.ChangeState(_ctx.CreateState(AbilityPhase.Exiting)); break;
                }
                return;
            }
            _ctx.OwnerMachine.ChangeState(_ctx.CreateState(AbilityPhase.Exiting));
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _ctx.CurrentIteration++;
            _ctx.Services?.PublishIterationStarted();
            Release();
            _ctx.Services?.PublishReleasePerformed();
        }

        protected virtual void Release() { }
    }
}