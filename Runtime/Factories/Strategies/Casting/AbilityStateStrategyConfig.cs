using UnityEngine;
using UnityEngine.Playables;



namespace LOGIYGames.Abilities
{
    public enum AbilityStateExitType
    {
        Timer,
        AnimationEvent
    }
    /// <summary>
    /// Shared configuration for an ability state runtime strategy.
    /// Cancellation and interruption are configured independently for every state.
    /// </summary>
    public abstract class AbilityStateStrategyConfig : ScriptableObject
    {

        [Header("Presentation")]
        public AbilityAnimationData AnimationData;

        [Header("State Lifecicle")]
        public AbilityStateExitType ExitOn;

        public float StateDuration;
        public string AnimationExitEventName;

        [Header("Ability Lifecicle")]
        public bool AllowCancel = false;

        public bool IsInterruptible = false;

        [Header("Ability State Restrictions")]
        public bool BlockMovement = false;

        public abstract AbilityStrategyRuntime CreateRuntime(AbilityContext ctx);
    }
}
