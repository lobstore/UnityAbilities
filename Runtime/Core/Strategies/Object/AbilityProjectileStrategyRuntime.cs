using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class AbilityProjectileStrategyRuntime : AbilityObjectStrategyRuntime
    {
        private readonly AbilityProjectileStrategyFactory _cfg;
        public AbilityProjectileStrategyRuntime(AbilityProjectileStrategyFactory cfg) : base(cfg) { _cfg = cfg; }
        public override void Initialize(AbilityContext ctx)
        {
            base.Initialize(ctx);
            if (ctx.AbilityInstancePrefab.TryGetComponent(out Rigidbody rb))
            {
                rb.linearVelocity = ctx.Caster.transform.forward * _cfg.Speed;
            }

        }
    }
}