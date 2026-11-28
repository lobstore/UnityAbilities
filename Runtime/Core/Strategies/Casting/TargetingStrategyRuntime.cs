using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class TargetingStrategyRuntime : AbilityStrategyRuntime
    {
        protected readonly TargetingStrategyConfig _cfg;
        public TargetingStrategyRuntime(TargetingStrategyConfig cfg, AbilityContext context) : base(cfg, context) { _cfg = cfg; }

        public override void OnEnter()
        {
            base.OnEnter();
            _ctx.Targets = new();
            _ctx.InstantiationPivot = null;
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
        protected virtual void UpdateTargeting() {  }
    }
}
