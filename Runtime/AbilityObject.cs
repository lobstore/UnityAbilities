using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames.Abilities
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class AbilityObject : MonoBehaviour
    {
        public UnityEvent<Collision> Collided = new();
        [SerializeReference] private AbilityObjectStrategyFactory config;
        AbilityObjectStrategyRuntime _strategy;
        public virtual void Initialize(AbilityContext ctx)
        {
            _strategy = config.CreateRuntime(ctx);
            _strategy?.Initialize(this);
        }
        public void Update()
        {
            _strategy?.Update();
        }
        private void OnCollisionEnter(Collision collision)
        {
            Collided.Invoke(collision);
            _strategy?.OnCollisionEnter(collision);
        }
    }
}