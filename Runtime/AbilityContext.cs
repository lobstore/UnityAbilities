using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class AbilityTargetData
    {
        public List<GameObject> Targets;
        public Vector3 Position;
        public Vector3 Direction;
    }
    public class AbilityContext
    {
        public AbilityStateMachine OwnerMachine;
        public Ability Ability;
        public AbilityTargetData TargetingData;
        public GameObject Caster;
        public int CurrentIteration;
        public Transform CastOrigin;
    }
}
