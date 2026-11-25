using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Settings for selecting a target along the caster's forward direction.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Strategies/Targeting/Forward")]
    public class ForwardTargetingStrategyConfig : TargetingStrategyConfig
    {
        [Min(0f)] public float RaycastDistance = 50f;
        public LayerMask TargetLayers = ~0;

        public override IAbilityStateStrategy CreateRuntime() => new ForwardTargetingRuntime(this);
    }
}
