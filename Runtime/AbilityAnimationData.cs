using System;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public struct AbilityAnimationData
    {
        public string AnimationName;
        public float Crossfade;
        public float MotionSpeed;
        public bool UseRootMotion;
    }
}
