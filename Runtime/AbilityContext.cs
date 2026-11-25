using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Mutable context shared by all runtime strategies during one ability activation.
    /// Values such as Target and TargetPoint are populated or updated by the targeting strategy.
    /// </summary>
    public class AbilityContext
    {
        /// <summary>GameObject executing the ability.</summary>
        public GameObject Caster;

        /// <summary>Selected target, if the ability uses an object target. Can be null.</summary>
        public GameObject Target;

        /// <summary>World-space point selected for the ability. Strategies should define a fallback when no target is found.</summary>
        public Vector3 TargetPoint;

        /// <summary>Animator used for ability state animations. Can be null.</summary>
        public Animator Animator;

        /// <summary>Runtime wrapper and authoring data for the currently executing ability.</summary>
        public AbilityRuntime Ability;

        /// <summary>Origin transform used to position or orient released effects, projectiles, and hits.</summary>
        public Transform CastOrigin;

        /// <summary>State machine that owns this context and should be used to request state transitions.</summary>
        public AbilityStateMachine OwnerMachine;
    }
}
