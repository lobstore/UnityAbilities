using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class AbilityContext
    {
        public GameObject Caster;
        public GameObject Target;
        public Vector3 TargetPoint;
        /// <summary>Zero-based completed-release counter used by strategies with MaxIterations greater than one.</summary>
        public int ReleaseIteration;
        public Animator Animator;
        public AbilityRuntime Ability;
        public Transform CastOrigin;
        public AbilityStateMachine OwnerMachine;
    }
}
