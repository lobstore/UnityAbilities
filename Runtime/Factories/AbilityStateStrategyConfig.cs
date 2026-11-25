using UnityEngine;

namespace LOGIYGames.Abilities
{
    public abstract class AbilityStateStrategyConfig : ScriptableObject
    {
        public AbilityAnimationData AnimationData;
        public GameObject VFX;
        public AudioClip SFX;
        public float StateDuration;
        public bool ExitStateOnAnimationEvent = false;
        public string AnimationEventName;

        [Tooltip("Allow the active ability to be cancelled while it is in this state.")]
        public bool AllowCancel = false;

        [Min(0f)]
        [Tooltip("Minimum time in this state before cancellation is accepted.")]
        public float MinTimeBeforeCancel = 0f;

        [Tooltip("Allow another ability to interrupt the active ability while it is in this state.")]
        public bool IsInterruptible = false;

        public bool BlockMovement = false;

        public abstract IAbilityStateStrategy CreateRuntime();
    }
}
