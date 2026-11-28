using LOGIYGames.Abilities;
using UnityEngine;

namespace LOGIYGames
{
    public class AbilitiesController : MonoBehaviour
    {
        [SerializeField] AbilityDefinition Primary;
        [SerializeField] AbilityDefinition Secondary;
        [SerializeField] Transform CastOrigin;

        Ability Current = null;

        AbilityStateMachine StateMachine = null;


        public KeyCode PrimarySkill = KeyCode.Alpha1;
        public KeyCode SecondarySkill = KeyCode.Alpha2;

        private void Awake()
        {
            StateMachine = new AbilityStateMachine();
            StateMachine.Finished += () =>
            {
                Current = null;
            };
        }
        private void Update()
        {
            StateMachine?.Update();
            if(Input.GetKeyDown(PrimarySkill))
            {
                TryActivateAbility(Primary);
            }
            if(Input.GetKeyDown(SecondarySkill))
            {
                TryActivateAbility(Secondary);
            }
        }
        public bool TryActivateAbility(AbilityDefinition Next)
        {
            if (Current != null) return false;
            if (Next == null) return false;

            Current = new Ability(Next);
            AbilityContext context = new()
            { 
                CastOrigin = CastOrigin,
                Caster = gameObject,
                Ability = Current,
                OwnerMachine = StateMachine
            };


            var state = new AbilityState(context.Ability.Data.Warmup.CreateRuntime(context));
            StateMachine.ChangeState(state);
            return true;
        }
    }
}
