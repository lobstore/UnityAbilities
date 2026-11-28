using System;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    [Serializable]
    public class PointTargetingStrategyFactory : TargetingStrategyFactory
    {
        public float MaxRange = 10f;
        public LayerMask RaycastMask;
        public bool RotateToPoint = true;
        public override AbilityStrategyRuntime CreateRuntime(AbilityContext ctx)
        {
            return new PointTargetingStrategyRuntime(this, ctx);
        }
    }
}
