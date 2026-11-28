using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class AbilityProjectileStrategyRuntime : AbilityObjectStrategyRuntime
    {
        private readonly AbilityProjectileStrategyFactory _cfg;
        public AbilityProjectileStrategyRuntime(AbilityProjectileStrategyFactory cfg, AbilityContext ctx) : base(cfg, ctx) { _cfg = cfg; }
        public override void Initialize(AbilityObject @object)
        {
            base.Initialize(@object);
            if (@object.TryGetComponent(out Rigidbody rb))
            {
                rb.linearVelocity = _ctx.Caster.transform.forward * _cfg.Speed;
            }

        }
    }
}