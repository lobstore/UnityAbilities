using System;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public enum AbilityPhase { None, Warmup, Targeting, Casting, Release, Exiting }
    public enum AbilityEventType { AbilityStarted, StateEntered, StateExited, AnimationEvent, ReleasePerformed, IterationStarted, AbilityEnded, AbilityCancelled, AbilityInterrupted, AbilityLevelChanged, ObjectCollision }
    public enum AbilityEndReason { Completed, Cancelled, Interrupted }

    public readonly struct AbilityEvent
    {
        public AbilityEventType Type { get; }
        public AbilityPhase Phase { get; }
        public int Iteration { get; }
        public string EventName { get; }
        public AbilityEndReason EndReason { get; }
        public int PreviousLevel { get; }
        public int CurrentLevel { get; }
        public GameObject SourceObject { get; }
        public GameObject OtherObject { get; }
        public Vector3 Position { get; }

        public AbilityEvent(AbilityEventType type, AbilityPhase phase = AbilityPhase.None,
            int iteration = 0, string eventName = null,
            AbilityEndReason endReason = AbilityEndReason.Completed,
            int previousLevel = 0, int currentLevel = 0,
            GameObject sourceObject = null, GameObject otherObject = null,
            Vector3 position = default)
        {
            Type = type; Phase = phase; Iteration = iteration; EventName = eventName;
            EndReason = endReason; PreviousLevel = previousLevel; CurrentLevel = currentLevel;
            SourceObject = sourceObject; OtherObject = otherObject; Position = position;
        }
    }

    public sealed class AbilityEventDispatcher
    {
        public event Action<AbilityEvent> Published;
        public void Publish(in AbilityEvent abilityEvent) => Published?.Invoke(abilityEvent);
    }

    [Serializable]
    public abstract class AbilityModuleFactory
    {
        public abstract AbilityModuleRuntime CreateRuntime(AbilityContext context);
    }

    public abstract class AbilityModuleRuntime : IDisposable
    {
        protected AbilityContext Context { get; }
        protected AbilityModuleRuntime(AbilityContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public virtual void OnEvent(in AbilityEvent abilityEvent) { }
        public virtual void Tick(float deltaTime) { }
        public virtual void Dispose() { }
    }
}