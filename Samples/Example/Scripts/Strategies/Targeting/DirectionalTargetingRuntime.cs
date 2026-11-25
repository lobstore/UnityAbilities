using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class DirectionalTargetingRuntime : TargetingStrategyRuntime
    {
        private readonly DirectionalTargetingStrategyConfig _dirCfg;
        public DirectionalTargetingRuntime(DirectionalTargetingStrategyConfig cfg) : base(cfg) => _dirCfg = cfg;

        protected override void UpdateTargeting(AbilityContext ctx)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, _dirCfg.RaycastDistance, _dirCfg.TargetLayers))
            {
                ctx.TargetPoint = hit.point;
                ctx.Target = hit.collider.gameObject;
            }
            else ctx.TargetPoint = ray.GetPoint(_dirCfg.RaycastDistance);
        }
    }
}
