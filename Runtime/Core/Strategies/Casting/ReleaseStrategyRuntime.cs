using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class ReleaseStrategyRuntime : AbilityStrategyRuntime
    {
        private readonly ReleaseStrategyConfig _cfg;

        public ReleaseStrategyRuntime(ReleaseStrategyConfig cfg, AbilityContext context) : base(cfg, context) { _cfg = cfg; }

        public override void OnEnter()
        {
            base.OnEnter();
            Instantiate();
        }
        private void Instantiate()
        {
            if (_ctx.Ability.Data.Prefab != null)
            {
                Vector3 pos = _ctx.InstantiationPivot.position + _ctx.InstantiationPivot.rotation * _cfg.PrefabPositionOffset;
                _ctx.AbilityInstancePrefab = Object.Instantiate(_ctx.Ability.Data.Prefab, pos, _ctx.InstantiationPivot.rotation, null);
                _ctx.AbilityInstancePrefab.transform.localScale = _cfg.PrefabScale;
                var comp = _ctx.AbilityInstancePrefab.GetComponent<AbilityObject>();
                if (comp != null)
                {
                    comp.Initialize(_ctx);
                }
                else
                {
                    Object.Destroy(_ctx.AbilityInstancePrefab, _cfg.fallbackLifetime);
                }
            }
        }
        protected override void StateTransitionRules()
        {
            _ctx.OwnerMachine.ChangeState(new AbilityState(_ctx.Ability.Data.Exiting.CreateRuntime(_ctx)));
        }


    }
}
