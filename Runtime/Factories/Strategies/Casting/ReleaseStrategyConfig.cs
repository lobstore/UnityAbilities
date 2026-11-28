using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Strategies/Release/Default")]
    public class ReleaseStrategyConfig : AbilityStateStrategyConfig
    {
        [Header("Ability Prefab Control")]
        [Min(0)] public float fallbackLifetime = 5;
        public Vector3 PrefabPositionOffset;
        public Vector3 PrefabScale = Vector3.one;
        /// <summary>Creates a fresh runtime strategy for one ability activation.</summary>
        /// <returns>A new release strategy.</returns>
        public override AbilityStrategyRuntime CreateRuntime(AbilityContext ctx)
        {
            return new ReleaseStrategyRuntime(this, ctx);
        }
    }
}
