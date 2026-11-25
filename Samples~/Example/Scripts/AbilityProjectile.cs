using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames.Abilities
{
    public class AbilityProjectile : MonoBehaviour
    {
        public UnityEvent OnCollided = new();
        AbilityContext _ctx;
        public void Initialize(AbilityContext ctx)
        {
            _ctx = ctx;
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Проверка «свой/чужой», чтобы не ударить кастера
            if (collision.gameObject == _ctx.Caster) return;
            OnCollided.Invoke();
            Destroy(gameObject);
        }
    }
}
