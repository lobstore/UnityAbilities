using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public sealed class AbilityModuleHost : IDisposable
    {
        private readonly AbilityContext _context;
        private readonly AbilityEventDispatcher _dispatcher;
        private readonly GameObject _objectEventFilter;
        private readonly List<AbilityModuleRuntime> _runtimes = new();
        private bool _disposed;

        public AbilityModuleHost(IEnumerable<AbilityModuleFactory> factories, AbilityContext context,
            AbilityEventDispatcher dispatcher, GameObject objectEventFilter = null)
        {
            _context = context;
            _dispatcher = dispatcher;
            _objectEventFilter = objectEventFilter;
            if (_dispatcher != null) _dispatcher.Published += HandleEvent;
            if (factories == null) return;
            foreach (AbilityModuleFactory factory in factories) Add(factory);
        }

        public void Add(AbilityModuleFactory factory)
        {
            if (_disposed || factory == null) return;
            try
            {
                AbilityModuleRuntime runtime = factory.CreateRuntime(_context);
                if (runtime != null) _runtimes.Add(runtime);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, _context != null ? _context.Caster : null);
            }
        }

        public void Tick(float deltaTime)
        {
            if (_disposed) return;
            AbilityModuleRuntime[] snapshot = _runtimes.ToArray();
            foreach (AbilityModuleRuntime runtime in snapshot)
            {
                try { runtime?.Tick(deltaTime); }
                catch (Exception exception) { Debug.LogException(exception, _context != null ? _context.Caster : null); }
            }
        }

        private void HandleEvent(AbilityEvent abilityEvent)
        {
            if (_disposed) return;
            if (_objectEventFilter != null &&
                abilityEvent.Type == AbilityEventType.ObjectCollision &&
                abilityEvent.SourceObject != _objectEventFilter)
                return;

            AbilityModuleRuntime[] snapshot = _runtimes.ToArray();
            foreach (AbilityModuleRuntime runtime in snapshot)
            {
                try { runtime?.OnEvent(in abilityEvent); }
                catch (Exception exception) { Debug.LogException(exception, _context != null ? _context.Caster : null); }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_dispatcher != null) _dispatcher.Published -= HandleEvent;
            for (int i = _runtimes.Count - 1; i >= 0; i--)
            {
                try { _runtimes[i]?.Dispose(); }
                catch (Exception exception) { Debug.LogException(exception, _context != null ? _context.Caster : null); }
            }
            _runtimes.Clear();
        }
    }
}