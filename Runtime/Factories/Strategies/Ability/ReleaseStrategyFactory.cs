using System;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public enum RepeatAbilityFrom
    {
        Targeting,
        Casting,
        Release
    }
    [Serializable]
    public class ReleaseStrategyFactory : AbilityStateStrategyFactory
    {
        [Header("Ability Prefab Control")]
        [Min(0)] public float fallbackLifetime = 5;
        public Vector3 PrefabPositionOffset;
        public Vector3 PrefabScale = Vector3.one;

        public uint Repeat = 0;
        public RepeatAbilityFrom RepeatFrom;

        public override AbilityStrategyRuntime CreateRuntime(AbilityContext ctx) => new ReleaseStrategyRuntime(this, ctx);
    }
}
