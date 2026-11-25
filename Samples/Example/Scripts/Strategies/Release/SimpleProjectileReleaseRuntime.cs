using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class SimpleProjectileReleaseRuntime : ReleaseStrategyRuntime
    {
        private readonly SimpleProjectileReleaseConfig _cfg;
        public SimpleProjectileReleaseRuntime(SimpleProjectileReleaseConfig cfg) : base(cfg) => _cfg = cfg;

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
