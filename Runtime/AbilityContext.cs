using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class AbilityContext
    {
        public GameObject Caster;
        public GameObject Target;
        public Vector3 TargetPoint;
        public AbilityTargetingInput TargetingInput;
        public int ReleaseIteration;
        public Animator Animator;
        public AbilityRuntime Ability;
        public Transform CastOrigin;
        public AbilityStateMachine OwnerMachine;
    }
}
