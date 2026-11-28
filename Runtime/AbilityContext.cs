using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class AbilityContext
    {
        public AbilityStateMachine OwnerMachine;
        public Ability Ability;
        public GameObject Caster;
        public List<GameObject> Targets;
        public Transform CastOrigin;
        public Transform InstantiationPivot;
        public GameObject AbilityInstancePrefab;
    }
}
