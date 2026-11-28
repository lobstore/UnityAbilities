using System;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public class ReleaseStrategyFactory : AbilityStateStrategyFactory
    {
        [Header("Ability Prefab Control")]
        [Min(0)] public float fallbackLifetime = 5;
        public Vector3 PrefabPositionOffset;
        public Vector3 PrefabScale = Vector3.one;
        public override AbilityStrategyRuntime CreateRuntime(AbilityContext ctx) => new ReleaseStrategyRuntime(this, ctx);
    }
}
