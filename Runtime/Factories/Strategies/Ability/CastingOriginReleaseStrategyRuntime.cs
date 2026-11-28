using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class CastingOriginReleaseStrategyRuntime : ReleaseStrategyRuntime
    {
        CastingOriginReleaseStrategyFactory _cfg;
        public CastingOriginReleaseStrategyRuntime(CastingOriginReleaseStrategyFactory cfg, AbilityContext context) : base(cfg, context)
        {
            _cfg = cfg;
        }
        override protected void Release()
        {
            var prefab = _ctx.Ability.Data.Prefab;
            if (prefab == null) return;
            GameObject go = UnityEngine.Object.Instantiate(prefab, _ctx.CastOrigin.position + _ctx.CastOrigin.transform.rotation * _cfg.PrefabPositionOffset , _ctx.CastOrigin.transform.rotation, _cfg.ParentToCastingOrigin ? _ctx.CastOrigin.transform : null);
            go.transform.localScale = _ctx.Ability.Data.Release.PrefabScale;
            if (go.TryGetComponent(out AbilityObject AO))
            {
                AO.Initialize(_ctx);
            }
            else
            {
                var lifetime = _ctx.Ability.Data.Release.fallbackLifetime;
                GameObject.Destroy(go, lifetime);

            }
        }
    }
}
