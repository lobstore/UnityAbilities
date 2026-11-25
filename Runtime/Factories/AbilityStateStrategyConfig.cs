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
        public bool IsInterruptible = false;
        public bool BlockMovement = false;
        public abstract IAbilityStateStrategy CreateRuntime();
    }
}
