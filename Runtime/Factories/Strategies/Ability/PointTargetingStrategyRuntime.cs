using LOGIYGames.Abilities;
using UnityEngine;

namespace LOGIYGames
{
    public class PointTargetingStrategyRuntime : TargetingStrategyRuntime
    {
        PointTargetingStrategyFactory _cfg;
        GameObject preview;
        bool confirmed;
        public PointTargetingStrategyRuntime(PointTargetingStrategyFactory cfg, AbilityContext context) : base(cfg, context) { _cfg = cfg; }
        protected override void StateTransitionRules()
        {
            _ctx.OwnerMachine.ChangeState(new AbilityState(_ctx.Ability.Data.Casting.CreateRuntime(_ctx)));
        }
        public override void OnEnter()
        {
            base.OnEnter();
            preview = Object.Instantiate(_ctx.Ability.Data.Prefab, _ctx.Caster.transform.position, Quaternion.identity);
        }
        protected override bool StateExitCondition()
        {
            return confirmed;
        }
        protected override void UpdateTargeting()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPoint = _ctx.Caster.transform.position;
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _cfg.RaycastMask))
            {
                targetPoint = hit.point;
                Vector3 direction = targetPoint - _ctx.Caster.transform.position;

                if (direction.magnitude > _cfg.MaxRange)
                {
                    targetPoint = _ctx.Caster.transform.position + direction.normalized * _cfg.MaxRange;
                }
                if (preview != null)
                {
                    preview.transform.position = targetPoint;
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                _ctx.TargetingData.Position = targetPoint;

                confirmed = true;
            }

            if (_cfg.RotateToPoint)
            {
                Vector3 dir = targetPoint - _ctx.Caster.transform.position;
                dir.y = 0;
                _ctx.Caster.transform.rotation = Quaternion.LookRotation(dir);
            }

        }
        public override void OnExit()
        {
            base.OnExit();
            Object.Destroy(preview);
        }
    }
}
