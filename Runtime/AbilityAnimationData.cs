using System;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Animator configuration used when a state starts its animation.
    /// </summary>
    [Serializable]
    public struct AbilityAnimationData
    {
        /// <summary>Name or path of the Animator state passed to CrossFadeInFixedTime.</summary>
        public string AnimationName;

        /// <summary>Cross-fade duration in seconds.</summary>
        public float Crossfade;

        /// <summary>Reserved animation motion speed multiplier. The built-in strategies currently do not apply this value.</summary>
        public float MotionSpeed;

        /// <summary>Root-motion preference for the consuming character/controller. The built-in strategies currently do not apply this value.</summary>
        public bool UseRootMotion;
    }
}
