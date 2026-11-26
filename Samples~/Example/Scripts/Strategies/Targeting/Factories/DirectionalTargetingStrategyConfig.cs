using System;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Legacy config retained so existing Fireball example assets remain loadable.
    /// New assets should use PositionTargetingStrategyConfig.
    /// </summary>
    [Obsolete("Use PositionTargetingStrategyConfig.")]
    [CreateAssetMenu(menuName = "Abilities/Strategies/Targeting/Legacy Directional (Position Input)")]
    public class DirectionalTargetingStrategyConfig : TargetingStrategyConfig
    {
        // Retained for serialization compatibility with existing sample assets.
        [Obsolete("Use CameraPositionTargetingInputProvider settings instead.")]
        public float RaycastDistance = 50f;

        [Obsolete("Use CameraPositionTargetingInputProvider settings instead.")]
        public LayerMask TargetLayers;

        public override IAbilityStateStrategy CreateRuntime() => new PositionTargetingRuntime(this);
    }
}
