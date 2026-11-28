using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class TargetPositionReleaseStrategyRuntime : ReleaseStrategyRuntime
    {
        TargetPositionReleaseStrategyFactory _cfg;
        public TargetPositionReleaseStrategyRuntime(TargetPositionReleaseStrategyFactory cfg, AbilityContext context) : base(cfg, context)
        {
            _cfg = cfg;
        }
        override protected void Release()
        {
            var prefab = _ctx.Ability.Data.Prefab;
            if (prefab == null) return;
            Vector3 pos = _ctx.TargetingData.Position;
            Quaternion rot = Quaternion.LookRotation(pos - _ctx.Caster.transform.position);

            if (_cfg.ResetPitchRotation)
            {
                Vector3 euler = rot.eulerAngles;
                rot = Quaternion.Euler(0f, euler.y, 0f);
            }
            else 
            {
                Vector3 euler = rot.eulerAngles;
                rot = Quaternion.Euler(euler.x, euler.y, 0f);
            }
            GameObject go = UnityEngine.Object.Instantiate(prefab, pos, rot, null);
            go.transform.position += go.transform.rotation * _cfg.PrefabPositionOffset;
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
