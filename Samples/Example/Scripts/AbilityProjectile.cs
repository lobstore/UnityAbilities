using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Sample projectile component that stores its originating ability context
    /// and raises an event when it collides with an object other than its caster.
    /// </summary>
    public class AbilityProjectile : MonoBehaviour
    {
        /// <summary>Invoked after the projectile collides with a non-caster object.</summary>
        public UnityEvent OnCollided = new();

        private AbilityContext _ctx;

        /// <summary>
        /// Initializes the projectile with the ability context that spawned it.
        /// Call this immediately after instantiation and before collisions are processed.
        /// </summary>
        /// <param name="ctx">Context of the ability that created this projectile.</param>
        public void Initialize(AbilityContext ctx)
        {
            _ctx = ctx;
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Ignore the caster so the projectile cannot hit its owner immediately.
            if (collision.gameObject == _ctx.Caster)
                return;

            OnCollided.Invoke();
            Destroy(gameObject);
        }
    }
}
