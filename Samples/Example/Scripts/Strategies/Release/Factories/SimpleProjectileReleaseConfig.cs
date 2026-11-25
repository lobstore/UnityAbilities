using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Strategies/Release/SimpleProjectile")]
    public class SimpleProjectileReleaseConfig : ReleaseStrategyConfig
    {
        public GameObject ProjectilePrefab;
        public float Speed = 20f;
        public float LifeTime = 5f;

        public override IAbilityStateStrategy CreateRuntime() => new SimpleProjectileReleaseRuntime(this);
    }
}
