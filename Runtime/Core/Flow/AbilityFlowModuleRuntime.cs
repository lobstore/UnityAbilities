using System;
using System.Collections.Generic;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public abstract class AbilityFlowModuleFactory
    {
        public abstract AbilityFlowModuleRuntime CreateRuntime(AbilityContext context);
    }

    public abstract class AbilityFlowModuleRuntime : IDisposable
    {
        protected AbilityContext Context { get; }
        protected AbilityFlowModuleRuntime(AbilityContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public virtual bool TryResolveNextPhase(out AbilityPhase nextPhase)
        {
            nextPhase = AbilityPhase.None;
            return false;
        }
        public virtual void Dispose() { }
    }

    public sealed class AbilityFlowResolver : IDisposable
    {
        private readonly List<AbilityFlowModuleRuntime> _runtimes = new();
        public bool HasModules => _runtimes.Count > 0;

        public AbilityFlowResolver(IEnumerable<AbilityFlowModuleFactory> factories, AbilityContext context)
        {
            if (factories == null) return;
            foreach (AbilityFlowModuleFactory factory in factories)
            {
                if (factory == null) continue;
                AbilityFlowModuleRuntime runtime = factory.CreateRuntime(context);
                if (runtime != null) _runtimes.Add(runtime);
            }
        }

        public bool TryResolveNextPhase(out AbilityPhase nextPhase)
        {
            foreach (AbilityFlowModuleRuntime runtime in _runtimes)
                if (runtime.TryResolveNextPhase(out nextPhase)) return true;
            nextPhase = AbilityPhase.None;
            return false;
        }

        public void Dispose()
        {
            for (int i = _runtimes.Count - 1; i >= 0; i--) _runtimes[i]?.Dispose();
            _runtimes.Clear();
        }
    }
}