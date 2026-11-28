using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public enum AbilityEffectStackPolicy { Stack, Refresh, Ignore }

    public sealed class AbilityEffectContext
    {
        public GameObject Caster { get; }
        public GameObject Target { get; }
        public AbilityDefinition SourceAbility { get; }
        public int AbilityLevel { get; }
        public AbilityParameters Parameters { get; }

        public AbilityEffectContext(AbilityContext source, GameObject target)
        {
            Caster = source != null ? source.Caster : null;
            Target = target;
            SourceAbility = source != null && source.Ability != null ? source.Ability.Data : null;
            AbilityLevel = source != null ? source.AbilityLevel : 1;
            Parameters = source != null ? source.Parameters : null;
        }
    }

    public abstract class AbilityEffectDefinition : ScriptableObject
    {
        [SerializeField] private string _effectId = "Effect";
        [SerializeField, Min(0f)] private float _duration = 5f;
        [SerializeField] private AbilityEffectStackPolicy _stackPolicy = AbilityEffectStackPolicy.Refresh;
        public string EffectId => string.IsNullOrWhiteSpace(_effectId) ? name : _effectId;
        public float Duration => Mathf.Max(0f, _duration);
        public AbilityEffectStackPolicy StackPolicy => _stackPolicy;
        public abstract AbilityEffectRuntime CreateRuntime(AbilityEffectContext context);
    }

    public abstract class AbilityEffectRuntime : IDisposable
    {
        public virtual void OnApply() { }
        public virtual void Tick(float deltaTime) { }
        public virtual void OnRemove() { }
        public virtual void Dispose() => OnRemove();
    }

    public interface IAbilityEffectReceiver
    {
        AbilityEffectHandle ApplyEffect(AbilityEffectDefinition definition, AbilityEffectContext context);
    }

    public sealed class AbilityEffectHandle : IDisposable
    {
        private Action _release;
        public bool IsDisposed { get; private set; }
        internal AbilityEffectHandle(Action release) => _release = release;
        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            Action release = _release;
            _release = null;
            release?.Invoke();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AbilityEffectController : MonoBehaviour, IAbilityEffectReceiver
    {
        private sealed class ActiveEffect
        {
            public int Id;
            public AbilityEffectDefinition Definition;
            public AbilityEffectRuntime Runtime;
            public float Remaining;
            public int Owners;
        }

        private readonly List<ActiveEffect> _activeEffects = new();
        private int _nextId;

        public AbilityEffectHandle ApplyEffect(AbilityEffectDefinition definition, AbilityEffectContext context)
        {
            if (definition == null) return null;

            if (definition.StackPolicy != AbilityEffectStackPolicy.Stack)
            {
                ActiveEffect existing = _activeEffects.Find(effect => effect.Definition == definition);
                if (existing != null)
                {
                    if (definition.StackPolicy == AbilityEffectStackPolicy.Refresh)
                        existing.Remaining = definition.Duration;
                    if (definition.StackPolicy == AbilityEffectStackPolicy.Ignore)
                        return new AbilityEffectHandle(() => { });
                    existing.Owners++;
                    return new AbilityEffectHandle(() => ReleaseOwner(existing.Id));
                }
            }

            AbilityEffectRuntime runtime = definition.CreateRuntime(context);
            if (runtime == null) return null;
            ActiveEffect active = new ActiveEffect
            {
                Id = ++_nextId,
                Definition = definition,
                Runtime = runtime,
                Remaining = definition.Duration,
                Owners = 1
            };
            _activeEffects.Add(active);
            runtime.OnApply();
            return new AbilityEffectHandle(() => ReleaseOwner(active.Id));
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            for (int i = _activeEffects.Count - 1; i >= 0; i--)
            {
                ActiveEffect active = _activeEffects[i];
                active.Runtime?.Tick(deltaTime);
                if (active.Remaining <= 0f) continue;
                active.Remaining -= deltaTime;
                if (active.Remaining <= 0f) RemoveAt(i);
            }
        }

        private void ReleaseOwner(int id)
        {
            ActiveEffect active = _activeEffects.Find(effect => effect.Id == id);
            if (active == null) return;
            active.Owners = Mathf.Max(0, active.Owners - 1);
            if (active.Owners == 0) Remove(id);
        }

        private void Remove(int id)
        {
            for (int i = _activeEffects.Count - 1; i >= 0; i--)
                if (_activeEffects[i].Id == id) { RemoveAt(i); return; }
        }

        private void RemoveAt(int index)
        {
            ActiveEffect active = _activeEffects[index];
            _activeEffects.RemoveAt(index);
            active.Runtime?.Dispose();
        }

        private void OnDestroy()
        {
            for (int i = _activeEffects.Count - 1; i >= 0; i--)
                _activeEffects[i].Runtime?.Dispose();
            _activeEffects.Clear();
        }
    }

    [CreateAssetMenu(menuName = "Abilities/Effects/Timed Visual Effect")]
    public sealed class TimedVisualEffectDefinition : AbilityEffectDefinition
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private string _anchorName;
        [SerializeField] private Vector3 _localPosition;
        [SerializeField] private Vector3 _localEulerAngles;
        public override AbilityEffectRuntime CreateRuntime(AbilityEffectContext context)
            => new TimedVisualEffectRuntime(_prefab, _anchorName, _localPosition, _localEulerAngles, context);
    }

    public sealed class TimedVisualEffectRuntime : AbilityEffectRuntime
    {
        private readonly GameObject _prefab;
        private readonly string _anchorName;
        private readonly Vector3 _localPosition;
        private readonly Vector3 _localEulerAngles;
        private readonly AbilityEffectContext _context;
        private GameObject _instance;

        public TimedVisualEffectRuntime(GameObject prefab, string anchorName, Vector3 localPosition,
            Vector3 localEulerAngles, AbilityEffectContext context)
        {
            _prefab = prefab;
            _anchorName = anchorName;
            _localPosition = localPosition;
            _localEulerAngles = localEulerAngles;
            _context = context;
        }

        public override void OnApply()
        {
            if (_prefab == null || _context?.Target == null) return;
            Transform anchor = _context.Target.transform;
            if (!string.IsNullOrWhiteSpace(_anchorName))
            {
                Transform candidate = anchor.Find(_anchorName);
                if (candidate != null) anchor = candidate;
            }
            _instance = UnityEngine.Object.Instantiate(_prefab, anchor);
            _instance.transform.localPosition = _localPosition;
            _instance.transform.localRotation = Quaternion.Euler(_localEulerAngles);
        }

        public override void OnRemove()
        {
            if (_instance == null) return;
            if (Application.isPlaying) UnityEngine.Object.Destroy(_instance);
            else UnityEngine.Object.DestroyImmediate(_instance);
            _instance = null;
        }
    }
}