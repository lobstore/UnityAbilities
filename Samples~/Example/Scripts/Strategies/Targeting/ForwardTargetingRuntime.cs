using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Selects a point or target along the direction the caster is facing.
    /// This strategy does not read player input or depend on a Camera.
    /// </summary>
    public class ForwardTargetingRuntime : TargetingStrategyRuntime
    {
        private readonly ForwardTargetingStrategyConfig _forwardConfig;

        public ForwardTargetingRuntime(ForwardTargetingStrategyConfig config) : base(config)
        {
            _forwardConfig = config;
        }

        protected override void UpdateTargeting(AbilityContext ctx)
        {
            Transform origin = ctx.CastOrigin != null ? ctx.CastOrigin : ctx.Caster.transform;
            Vector3 direction = ctx.Caster.transform.forward.normalized;
            Ray ray = new Ray(origin.position, direction);

            if (Physics.Raycast(ray, out RaycastHit hit, _forwardConfig.RaycastDistance, _forwardConfig.TargetLayers))
            {
                ctx.Target = hit.collider.gameObject;
                ctx.TargetPoint = hit.point;
            }
            else
            {
                ctx.Target = null;
                ctx.TargetPoint = ray.GetPoint(_forwardConfig.RaycastDistance);
            }
        }
    }
}
