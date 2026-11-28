using System;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public enum AbilitySpawnLocation { Caster, CastOrigin, TargetPosition, TargetObject }

    [Serializable]
    public abstract class AbilityPrefabModuleFactory : AbilityModuleFactory
    {
        public GameObject Prefab;
        public AbilityEventType Trigger = AbilityEventType.StateEntered;
        public bool FilterByPhase = true;
        public AbilityPhase Phase = AbilityPhase.Casting;
        public AbilitySpawnLocation Location = AbilitySpawnLocation.CastOrigin;
        public string AnchorName;
        public bool ParentToAnchor;
        public Vector3 LocalPosition;
        public Vector3 LocalEulerAngles;
        [Min(0f)] public float LifetimeSeconds = 3f;
        public AbilityResourceScope ResourceScope = AbilityResourceScope.State;
        public override AbilityModuleRuntime CreateRuntime(AbilityContext context)
            => new SpawnPrefabModuleRuntime(this, context);
    }

    [Serializable]
    public sealed class SpawnVFXModuleFactory : AbilityPrefabModuleFactory
    {
        public SpawnVFXModuleFactory()
        {
            Trigger = AbilityEventType.StateEntered;
            Phase = AbilityPhase.Casting;
            Location = AbilitySpawnLocation.CastOrigin;
            LifetimeSeconds = 3f;
            ResourceScope = AbilityResourceScope.State;
        }
    }

    [Serializable]
    public sealed class SpawnAttachedObjectModuleFactory : AbilityPrefabModuleFactory
    {
        public SpawnAttachedObjectModuleFactory()
        {
            Trigger = AbilityEventType.AbilityStarted;
            Phase = AbilityPhase.None;
            FilterByPhase = false;
            Location = AbilitySpawnLocation.Caster;
            ParentToAnchor = true;
            LifetimeSeconds = 0f;
            ResourceScope = AbilityResourceScope.Ability;
        }
    }

    public sealed class SpawnPrefabModuleRuntime : AbilityModuleRuntime
    {
        private readonly AbilityPrefabModuleFactory _factory;
        public SpawnPrefabModuleRuntime(AbilityPrefabModuleFactory factory, AbilityContext context) : base(context)
            => _factory = factory;

        public override void OnEvent(in AbilityEvent abilityEvent)
        {
            if (_factory.Prefab == null || abilityEvent.Type != _factory.Trigger ||
                (_factory.FilterByPhase && abilityEvent.Phase != _factory.Phase)) return;

            GameObject instance = AbilitySpawnUtility.Spawn(Context, _factory.Prefab, _factory.Location,
                _factory.AnchorName, _factory.ParentToAnchor, _factory.LocalPosition, _factory.LocalEulerAngles);
            if (instance == null) return;
            if (_factory.LifetimeSeconds > 0f) UnityEngine.Object.Destroy(instance, _factory.LifetimeSeconds);

            if (_factory.ResourceScope != AbilityResourceScope.Detached && Context.Services != null)
                Context.Services.Resources.Register(instance, _factory.ResourceScope,
                    _factory.ResourceScope == AbilityResourceScope.State ? Context.Services.CurrentStateId : 0);
        }
    }

    [Serializable]
    public sealed class PlaySFXModuleFactory : AbilityModuleFactory
    {
        public AudioClip Clip;
        public AudioSource Source;
        public AbilityEventType Trigger = AbilityEventType.StateEntered;
        public bool FilterByPhase = true;
        public AbilityPhase Phase = AbilityPhase.Casting;
        [Range(0f, 1f)] public float Volume = 1f;
        [Range(0f, 1f)] public float SpatialBlend = 1f;
        public AbilityResourceScope ResourceScope = AbilityResourceScope.State;
        public override AbilityModuleRuntime CreateRuntime(AbilityContext context)
            => new PlaySFXModuleRuntime(this, context);
    }

    public sealed class PlaySFXModuleRuntime : AbilityModuleRuntime
    {
        private readonly PlaySFXModuleFactory _factory;
        public PlaySFXModuleRuntime(PlaySFXModuleFactory factory, AbilityContext context) : base(context)
            => _factory = factory;

        public override void OnEvent(in AbilityEvent abilityEvent)
        {
            if (_factory.Clip == null || abilityEvent.Type != _factory.Trigger ||
                (_factory.FilterByPhase && abilityEvent.Phase != _factory.Phase)) return;

            if (_factory.Source != null)
            {
                _factory.Source.PlayOneShot(_factory.Clip, _factory.Volume);
                return;
            }
            if (Context.Caster == null) return;

            GameObject audioObject = new GameObject("Ability SFX");
            audioObject.transform.position = Context.Caster.transform.position;
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.clip = _factory.Clip;
            audioSource.volume = _factory.Volume;
            audioSource.spatialBlend = _factory.SpatialBlend;
            audioSource.loop = false;
            audioSource.Play();
            UnityEngine.Object.Destroy(audioObject, _factory.Clip.length + 0.1f);

            if (_factory.ResourceScope != AbilityResourceScope.Detached && Context.Services != null)
                Context.Services.Resources.Register(audioObject, _factory.ResourceScope,
                    _factory.ResourceScope == AbilityResourceScope.State ? Context.Services.CurrentStateId : 0);
        }
    }

    internal static class AbilitySpawnUtility
    {
        public static GameObject Spawn(AbilityContext context, GameObject prefab, AbilitySpawnLocation location,
            string anchorName, bool parentToAnchor, Vector3 localPosition, Vector3 localEulerAngles)
        {
            if (context == null || prefab == null) return null;
            Transform anchor = ResolveAnchor(context, location, anchorName);
            Vector3 position;
            Quaternion rotation;

            if (location == AbilitySpawnLocation.TargetPosition)
            {
                position = context.TargetingData != null ? context.TargetingData.Position :
                    context.Caster != null ? context.Caster.transform.position : Vector3.zero;
                Vector3 direction = context.TargetingData != null ? context.TargetingData.Direction : Vector3.zero;
                rotation = direction.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(direction.normalized) : Quaternion.identity;
            }
            else
            {
                position = anchor != null ? anchor.position :
                    context.Caster != null ? context.Caster.transform.position : Vector3.zero;
                rotation = anchor != null ? anchor.rotation : Quaternion.identity;
            }

            Quaternion localRotation = Quaternion.Euler(localEulerAngles);
            GameObject instance = UnityEngine.Object.Instantiate(prefab, position, rotation, parentToAnchor ? anchor : null);
            if (parentToAnchor && anchor != null)
            {
                instance.transform.localPosition = localPosition;
                instance.transform.localRotation = localRotation;
            }
            else
            {
                instance.transform.position += rotation * localPosition;
                instance.transform.rotation = rotation * localRotation;
            }
            return instance;
        }

        private static Transform ResolveAnchor(AbilityContext context, AbilitySpawnLocation location, string anchorName)
        {
            Transform root = null;
            switch (location)
            {
                case AbilitySpawnLocation.CastOrigin:
                    root = context.CastOrigin != null ? context.CastOrigin :
                        context.Caster != null ? context.Caster.transform : null;
                    break;
                case AbilitySpawnLocation.TargetObject:
                    if (context.TargetingData?.Targets != null && context.TargetingData.Targets.Count > 0 &&
                        context.TargetingData.Targets[0] != null)
                        root = context.TargetingData.Targets[0].transform;
                    break;
                case AbilitySpawnLocation.Caster:
                    root = context.Caster != null ? context.Caster.transform : null;
                    break;
            }

            if (root == null || string.IsNullOrWhiteSpace(anchorName)) return root;
            Transform child = root.Find(anchorName);
            if (child != null) return child;
            foreach (Transform candidate in root.GetComponentsInChildren<Transform>(true))
                if (candidate.name == anchorName) return candidate;
            return root;
        }
    }
}