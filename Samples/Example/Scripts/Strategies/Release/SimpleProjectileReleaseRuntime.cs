using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Sample Release strategy that spawns a projectile from the caster's cast origin
    /// and launches it toward the target point.
    /// </summary>
    public class SimpleProjectileReleaseRuntime : ReleaseStrategyRuntime
    {
        private readonly SimpleProjectileReleaseConfig _cfg;

        /// <summary>
        /// Creates a projectile release runtime using the supplied projectile configuration.
        /// </summary>
        /// <param name="cfg">Projectile prefab, speed, and lifetime settings.</param>
        public SimpleProjectileReleaseRuntime(SimpleProjectileReleaseConfig cfg) : base(cfg) => _cfg = cfg;

        /// <summary>
        /// Runs the base Release initialization, spawns the projectile, assigns its velocity,
        /// and passes the current ability context to its collision handler.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        public override void OnEnter(AbilityContext ctx)
        {
            base.OnEnter(ctx);
            Vector3 dir = (ctx.TargetPoint - ctx.CastOrigin.position);
            dir = dir.sqrMagnitude > 0.0001f ? dir.normalized : ctx.Caster.transform.forward;

            var go = UnityEngine.Object.Instantiate(_cfg.ProjectilePrefab, ctx.CastOrigin.position, Quaternion.LookRotation(dir));

            var rb = go.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = dir * _cfg.Speed;

            go.GetComponent<AbilityProjectile>()?.Initialize(ctx);
            UnityEngine.Object.Destroy(go, _cfg.LifeTime);
        }
    }
}
