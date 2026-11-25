using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// ScriptableObject settings for the sample projectile Release strategy.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Strategies/Release/SimpleProjectile")]
    public class SimpleProjectileReleaseConfig : ReleaseStrategyConfig
    {
        /// <summary>Projectile prefab instantiated when Release begins.</summary>
        public GameObject ProjectilePrefab;

        /// <summary>Initial projectile speed in world units per second.</summary>
        public float Speed = 20f;

        /// <summary>Projectile lifetime in seconds before it is automatically destroyed.</summary>
        public float LifeTime = 5f;

        /// <summary>Creates a new projectile Release runtime for one ability activation.</summary>
        /// <returns>A fresh <see cref="SimpleProjectileReleaseRuntime"/> instance.</returns>
        public override IAbilityStateStrategy CreateRuntime() => new SimpleProjectileReleaseRuntime(this);
    }
}
