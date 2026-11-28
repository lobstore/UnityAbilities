using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames.Abilities
{
    public class AbilityObject : MonoBehaviour
    {
        public UnityEvent<Collision> Collided = new();
        [SerializeReference] private AbilityObjectStrategyFactory config;
        AbilityObjectStrategyRuntime _strategy;
        public virtual void Initialize(AbilityContext ctx)
        {
            _strategy = config.CreateRuntime();
            _strategy?.Initialize(ctx);
        }
        public void Update()
        {
            _strategy?.Update();
        }
        private void OnCollisionEnter(Collision collision)
        {
            Collided.Invoke(collision);
            _strategy.OnCollisionEnter(collision);
        }
        private void OnDestroy()
        {
            _strategy.Dispose();
        }
    }
}