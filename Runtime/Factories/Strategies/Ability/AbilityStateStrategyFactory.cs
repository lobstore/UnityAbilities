using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public enum AbilityStateExitType { Timer, AnimationEvent }

    [Serializable]
    public abstract class AbilityStateStrategyFactory
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
        [Header("State Modules")]
        [SerializeReference] public List<AbilityModuleFactory> Modules = new();
        public abstract AbilityStrategyRuntime CreateRuntime(AbilityContext ctx);
    }
}