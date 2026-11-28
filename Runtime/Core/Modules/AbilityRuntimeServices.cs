using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public sealed class AbilityRuntimeServices
    {
        private readonly AbilityContext _context;
        private AbilityModuleHost _stateModules;
        private int _nextStateId;
        private bool _abilityEnded;
        private bool _initialized;

        public AbilityEventDispatcher Events { get; }
        public AbilityResourceRegistry Resources { get; }
        public AbilityModuleHost AbilityModules { get; private set; }
        public AbilityFlowResolver Flow { get; private set; }
        public int CurrentStateId { get; private set; }

        public AbilityRuntimeServices(AbilityContext context)
        {
            _context = context;
            Events = new AbilityEventDispatcher();
            Resources = new AbilityResourceRegistry();
        }

        public void Initialize()
        {
            if (_initialized || _context.Ability == null || _context.Ability.Data == null) return;
            _initialized = true;
            AbilityDefinition definition = _context.Ability.Data;
            List<AbilityModuleFactory> factories = new();
            if (definition.Modules != null) factories.AddRange(definition.Modules);

            if (definition.Progression != null && definition.Progression.Levels != null)
            {
                foreach (AbilityLevelDefinition level in definition.Progression.Levels)
                {
                    if (level == null || level.Level > _context.AbilityLevel || level.Modules == null) continue;
                    factories.AddRange(level.Modules);
                }
            }

            AbilityModules = new AbilityModuleHost(factories, _context, Events);
            Flow = new AbilityFlowResolver(definition.FlowModules, _context);
            Events.Publish(new AbilityEvent(AbilityEventType.AbilityStarted,
                iteration: _context.CurrentIteration, currentLevel: _context.AbilityLevel));
        }

        public int BeginState(AbilityPhase phase, AbilityStateStrategyFactory factory)
        {
            CurrentStateId = ++_nextStateId;
            _stateModules?.Dispose();
            _stateModules = new AbilityModuleHost(factory != null ? factory.Modules : null, _context, Events);
            Events.Publish(new AbilityEvent(AbilityEventType.StateEntered, phase, _context.CurrentIteration,
                currentLevel: _context.AbilityLevel));
            return CurrentStateId;
        }

        public void EndState(AbilityPhase phase, int stateId)
        {
            Events.Publish(new AbilityEvent(AbilityEventType.StateExited, phase, _context.CurrentIteration,
                currentLevel: _context.AbilityLevel));
            _stateModules?.Dispose();
            _stateModules = null;
            Resources.ReleaseScope(AbilityResourceScope.State, stateId);
        }

        public void Tick(float deltaTime)
        {
            if (_abilityEnded) return;
            AbilityModules?.Tick(deltaTime);
            _stateModules?.Tick(deltaTime);
        }

        public void PublishAnimationEvent(AbilityPhase phase, string eventName)
        {
            Events.Publish(new AbilityEvent(AbilityEventType.AnimationEvent, phase,
                _context.CurrentIteration, eventName, currentLevel: _context.AbilityLevel));
        }
        public void PublishReleasePerformed()
        {
            Events.Publish(new AbilityEvent(AbilityEventType.ReleasePerformed, AbilityPhase.Release,
                _context.CurrentIteration, currentLevel: _context.AbilityLevel));
        }
        public void PublishIterationStarted()
        {
            Events.Publish(new AbilityEvent(AbilityEventType.IterationStarted, AbilityPhase.Release,
                _context.CurrentIteration, currentLevel: _context.AbilityLevel));
        }
        public void PublishObjectCollision(GameObject sourceObject, GameObject otherObject, Vector3 position)
        {
            Events.Publish(new AbilityEvent(AbilityEventType.ObjectCollision,
                iteration: _context.CurrentIteration, currentLevel: _context.AbilityLevel,
                sourceObject: sourceObject, otherObject: otherObject, position: position));
        }

        public void OnAbilityLevelChanged(int previousLevel, int currentLevel)
        {
            if (AbilityModules == null || _context.Ability == null ||
                _context.Ability.Data.Progression == null) return;
            for (int level = previousLevel + 1; level <= currentLevel; level++)
            {
                AbilityLevelDefinition levelDefinition = _context.Ability.Data.Progression.GetLevel(level);
                if (levelDefinition?.Modules == null) continue;
                foreach (AbilityModuleFactory factory in levelDefinition.Modules) AbilityModules.Add(factory);
            }
            Events.Publish(new AbilityEvent(AbilityEventType.AbilityLevelChanged,
                iteration: _context.CurrentIteration, previousLevel: previousLevel, currentLevel: currentLevel));
        }

        public void EndAbility(AbilityEndReason reason)
        {
            if (_abilityEnded) return;
            _abilityEnded = true;
            AbilityEventType reasonEvent = reason switch
            {
                AbilityEndReason.Cancelled => AbilityEventType.AbilityCancelled,
                AbilityEndReason.Interrupted => AbilityEventType.AbilityInterrupted,
                _ => AbilityEventType.AbilityEnded
            };
            Events.Publish(new AbilityEvent(reasonEvent, iteration: _context.CurrentIteration,
                endReason: reason, currentLevel: _context.AbilityLevel));
            if (reason != AbilityEndReason.Completed)
                Events.Publish(new AbilityEvent(AbilityEventType.AbilityEnded,
                    iteration: _context.CurrentIteration, endReason: reason, currentLevel: _context.AbilityLevel));

            _stateModules?.Dispose();
            _stateModules = null;
            AbilityModules?.Dispose();
            AbilityModules = null;
            Flow?.Dispose();
            Flow = null;
            Resources.ReleaseAll();
        }
    }
}