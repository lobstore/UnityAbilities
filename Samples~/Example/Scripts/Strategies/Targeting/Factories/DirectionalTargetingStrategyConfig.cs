using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Strategies/Targeting/Directional")]
    public class DirectionalTargetingStrategyConfig : TargetingStrategyConfig
    {
        public float RaycastDistance = 50f;
        public LayerMask TargetLayers;

        public override IAbilityStateStrategy CreateRuntime() => new DirectionalTargetingRuntime(this);
    }
}
