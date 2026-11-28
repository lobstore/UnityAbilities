using System;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class AbilityObjectStrategyRuntime
    {
        AbilityObjectStrategyFactory _cfg;
        protected AbilityContext _ctx;
        protected AbilityObject _abilityObject;
        public AbilityObjectStrategyRuntime(AbilityObjectStrategyFactory cfg, AbilityContext ctx)
        {
            _cfg = cfg;
            _ctx = ctx;
        }
        public virtual void Initialize(AbilityObject @object)
        {
            _abilityObject = @object;
            UnityEngine.Object.Destroy(@object.gameObject, _cfg.LifeTime);
        }
        public virtual void Update() { }
        public virtual void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject == _ctx.Caster) return;
            UnityEngine.Object.Destroy(_abilityObject.gameObject, _cfg.LifeTime);
        }
        public virtual void OnCollisionStay(){}

    }
}