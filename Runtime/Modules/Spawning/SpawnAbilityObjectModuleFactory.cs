using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public sealed class SpawnAbilityObjectModuleFactory : AbilityModuleFactory
    {
        public GameObject Prefab;
        public AbilityEventType Trigger = AbilityEventType.ReleasePerformed;
        public bool FilterByPhase;
        public AbilityPhase Phase = AbilityPhase.Release;
        public AbilitySpawnLocation Location = AbilitySpawnLocation.CastOrigin;
        public string AnchorName;
        public Vector3 PositionOffset;
        public Vector3 RotationOffset;
        [Min(0f)] public float LifetimeSeconds = 8f;
        public AbilityResourceScope ResourceScope = AbilityResourceScope.Detached;
        public List<AbilityEffectDefinition> InitialEffects = new();

        public override AbilityModuleRuntime CreateRuntime(AbilityContext context)
            => new SpawnAbilityObjectModuleRuntime(this, context);
    }

    public sealed class SpawnAbilityObjectModuleRuntime : AbilityModuleRuntime
    {
        private readonly SpawnAbilityObjectModuleFactory _factory;
        public SpawnAbilityObjectModuleRuntime(SpawnAbilityObjectModuleFactory factory, AbilityContext context) : base(context)
            => _factory = factory;

        public override void OnEvent(in AbilityEvent abilityEvent)
        {
            if (_factory.Prefab == null || abilityEvent.Type != _factory.Trigger ||
                (_factory.FilterByPhase && abilityEvent.Phase != _factory.Phase)) return;

            GameObject instance = AbilitySpawnUtility.Spawn(Context, _factory.Prefab, _factory.Location,
                _factory.AnchorName, false, _factory.PositionOffset, _factory.RotationOffset);
            if (instance == null) return;

            if (instance.TryGetComponent(out AbilityObject abilityObject))
                abilityObject.Initialize(new AbilityObjectContext(Context, _factory.InitialEffects));

            if (_factory.LifetimeSeconds > 0f)
                UnityEngine.Object.Destroy(instance, _factory.LifetimeSeconds);

            if (_factory.ResourceScope != AbilityResourceScope.Detached && Context.Services != null)
                Context.Services.Resources.Register(instance, _factory.ResourceScope,
                    _factory.ResourceScope == AbilityResourceScope.State ? Context.Services.CurrentStateId : 0);
        }
    }
}