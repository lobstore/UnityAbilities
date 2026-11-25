using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Base ScriptableObject configuration for an ability phase.
    /// Shared properties define animation, audiovisual effects, timing,
    /// cancellation, and interruption behavior.
    /// </summary>
    public abstract class AbilityStateStrategyConfig : ScriptableObject
    {
        /// <summary>Animator state name, cross-fade duration, motion speed, and root-motion preference for this phase.</summary>
        public AbilityAnimationData AnimationData;

        /// <summary>Optional prefab instantiated when this phase is entered.</summary>
        public GameObject VFX;

        /// <summary>Optional audio clip played at the caster's position when this phase is entered.</summary>
        public AudioClip SFX;

        /// <summary>Maximum phase duration in seconds. The concrete strategy defines how this value is used.</summary>
        public float StateDuration;

        /// <summary>When enabled, the concrete strategy may leave the phase after receiving its configured animation event.</summary>
        public bool ExitStateOnAnimationEvent = false;

        /// <summary>Exact Animator event name used with <see cref="ExitStateOnAnimationEvent"/>.</summary>
        public string AnimationEventName;

        /// <summary>Whether explicit cancellation is permitted while the ability is in this phase.</summary>
        [Tooltip("Allow the active ability to be cancelled while it is in this state.")]
        public bool AllowCancel = false;

        /// <summary>Minimum time in seconds that must elapse after entering this phase before cancellation is accepted.</summary>
        [Min(0f)]
        [Tooltip("Minimum time in this state before cancellation is accepted.")]
        public float MinTimeBeforeCancel = 0f;

        /// <summary>Whether a different ability is allowed to interrupt the active ability during this phase.</summary>
        [Tooltip("Allow another ability to interrupt the active ability while it is in this state.")]
        public bool IsInterruptible = false;

        /// <summary>Whether the owning character controller should block movement during this phase.</summary>
        public bool BlockMovement = false;

        /// <summary>
        /// Creates a fresh runtime strategy instance configured by this asset.
        /// Mutable execution state should be stored in the returned object, not in this asset.
        /// </summary>
        /// <returns>A new runtime implementation of <see cref="IAbilityStateStrategy"/>.</returns>
        public abstract IAbilityStateStrategy CreateRuntime();
    }
}
