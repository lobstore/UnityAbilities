using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Sample Targeting strategy that raycasts from the main camera through the mouse cursor.
    /// </summary>
    public class DirectionalTargetingRuntime : TargetingStrategyRuntime
    {
        private readonly DirectionalTargetingStrategyConfig _dirCfg;

        /// <summary>
        /// Creates directional targeting using the supplied raycast settings.
        /// </summary>
        /// <param name="cfg">Raycast distance and target layer configuration.</param>
        public DirectionalTargetingRuntime(DirectionalTargetingStrategyConfig cfg) : base(cfg) => _dirCfg = cfg;

        /// <summary>
        /// Updates the ability target from the mouse ray. When no collider is hit,
        /// uses the point at the configured maximum ray distance and clears the object target.
        /// </summary>
        /// <param name="ctx">Shared context whose Target and TargetPoint values are updated.</param>
        protected override void UpdateTargeting(AbilityContext ctx)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, _dirCfg.RaycastDistance, _dirCfg.TargetLayers))
            {
                ctx.TargetPoint = hit.point;
                ctx.Target = hit.collider.gameObject;
            }
            else
            {
                ctx.Target = null;
                ctx.TargetPoint = ray.GetPoint(_dirCfg.RaycastDistance);
            }
        }
    }
}
