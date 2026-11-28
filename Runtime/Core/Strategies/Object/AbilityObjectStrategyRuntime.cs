using System;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class AbilityObjectStrategyRuntime : IDisposable
    {
        AbilityObjectStrategyFactory _cfg;
        protected AbilityContext _ctx;
        public AbilityObjectStrategyRuntime(AbilityObjectStrategyFactory cfg)
        {
            _cfg = cfg;
        }
        public virtual void Initialize(AbilityContext ctx)
        {
            _ctx = ctx;
            UnityEngine.Object.Destroy(_ctx.AbilityInstancePrefab, _cfg.LifeTime);
        }
        public virtual void Update() { }
        public virtual void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject == _ctx.Caster && !_ctx.Targets.Contains(collision.gameObject)) return;
            UnityEngine.Object.Destroy(_ctx.AbilityInstancePrefab, _cfg.LifeTime);
        }
        public virtual void OnCollisionStay(){}

        public virtual void Dispose() { }
    }
}