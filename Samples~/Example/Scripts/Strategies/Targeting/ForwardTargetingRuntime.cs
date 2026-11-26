using System;
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

        /// <summary>Creates a forward-targeting runtime using the supplied raycast settings.</summary>
        public ForwardTargetingRuntime(ForwardTargetingStrategyConfig config) : base(config)
        {
            _forwardConfig = config;
        }

        /// <summary>Raycasts along the caster's forward vector and updates Target/TargetPoint.</summary>
        protected override void UpdateTargeting(AbilityContext ctx)
        {
            Transform origin = ctx.CastOrigin != null ? ctx.CastOrigin : ctx.Caster.transform;
            Vector3 direction = ctx.Caster.transform.forward.normalized;
            Ray ray = new Ray(origin.position, direction);

            RaycastHit[] hits = Physics.RaycastAll(
                ray,
                _forwardConfig.RaycastDistance,
                _forwardConfig.TargetLayers);

            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                // Ignore every collider belonging to the caster hierarchy.
                if (hit.collider.transform.root == ctx.Caster.transform.root)
                    continue;

                ctx.Target = hit.collider.gameObject;
                ctx.TargetPoint = hit.point;
                return;
            }

            ctx.Target = null;
            ctx.TargetPoint = ray.GetPoint(_forwardConfig.RaycastDistance);
        }
    }
}
