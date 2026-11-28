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
        private AbilityObjectStrategyRuntime _strategy;
        private AbilityModuleHost _moduleHost;
        private AbilityContext _sourceContext;

        public virtual void Initialize(AbilityContext ctx) => Initialize(new AbilityObjectContext(ctx));

        public virtual void Initialize(AbilityObjectContext context)
        {
            _sourceContext = context != null ? context.SourceContext : null;
            if (_sourceContext == null) return;

            if (context.InitialEffects != null && context.InitialEffects.Count > 0)
            {
                AbilityEffectController effectController = GetComponent<AbilityEffectController>();
                if (effectController == null)
                    effectController = gameObject.AddComponent<AbilityEffectController>();

                foreach (AbilityEffectDefinition effect in context.InitialEffects)
                    effectController.ApplyEffect(effect, new AbilityEffectContext(_sourceContext, gameObject));
            }

            if (config == null) return;
            _strategy = config.CreateRuntime(_sourceContext);
            _strategy?.Initialize(this);
            _moduleHost = new AbilityModuleHost(config.Modules, _sourceContext,
                _sourceContext.Services != null ? _sourceContext.Services.Events : null, gameObject);
        }

        private void Update()
        {
            _strategy?.Update();
            _moduleHost?.Tick(Time.deltaTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Collided.Invoke(collision);
            if (collision != null)
            {
                Vector3 point = collision.contactCount > 0 ? collision.GetContact(0).point : transform.position;
                _sourceContext?.Services?.PublishObjectCollision(gameObject, collision.gameObject, point);
            }
            _strategy?.OnCollisionEnter(collision);
        }

        private void OnDestroy()
        {
            _moduleHost?.Dispose();
            _moduleHost = null;
        }
    }
}