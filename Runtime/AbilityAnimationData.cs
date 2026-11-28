using System;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public struct AbilityAnimationData
    {
        public string AnimationName;
        [Min(0)] public float Crossfade;
        [Min(0)] public float MotionSpeed;
        public bool UseRootMotion;
        public uint layerIndex;
    }
}
