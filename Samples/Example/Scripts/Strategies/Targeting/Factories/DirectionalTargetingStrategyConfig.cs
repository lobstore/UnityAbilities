using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// ScriptableObject settings for mouse/camera raycast Targeting.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Strategies/Targeting/Directional")]
    public class DirectionalTargetingStrategyConfig : TargetingStrategyConfig
    {
        /// <summary>Maximum distance of the raycast and fallback target point.</summary>
        public float RaycastDistance = 50f;

        /// <summary>Layers that may be selected by the targeting raycast.</summary>
        public LayerMask TargetLayers;

        /// <summary>Creates a new directional Targeting runtime for one ability activation.</summary>
        /// <returns>A fresh <see cref="DirectionalTargetingRuntime"/> instance.</returns>
        public override IAbilityStateStrategy CreateRuntime() => new DirectionalTargetingRuntime(this);
    }
}
