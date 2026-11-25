using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class AbilityController : MonoBehaviour
    {
        [Header("Loadout")]
        [Tooltip("Клавиша 1 — например, Огненный шар")]
        public AbilityData PrimaryAbility;

        [Tooltip("Клавиша 2 — например, Метеор")]
        public AbilityData SecondaryAbility;

        [Header("References")]
        public Animator Animator;
        public Transform CastOrigin;

        private AbilityStateMachine _stateMachine;
        private AnimationEventReceiver _animReceiver;

        public bool IsCasting => _stateMachine != null && !_stateMachine.IsFinished;
        private List<AbilityRuntime> _abilities = new();
        public IReadOnlyList<AbilityRuntime> Abilities => _abilities;

        AbilityRuntime _currentAbility;

        private void Awake()
        {
            _abilities.Add(new AbilityRuntime(PrimaryAbility));
            _abilities.Add(new AbilityRuntime(SecondaryAbility));
            if (Animator == null) Animator = GetComponent<Animator>();
            if (CastOrigin == null) CastOrigin = transform;

            _animReceiver = GetComponent<AnimationEventReceiver>();
            if (_animReceiver != null)
                _animReceiver.OnAnimationEvent += HandleAnimationEvent;
        }

        private void OnDestroy()
        {
            if (_animReceiver != null)
                _animReceiver.OnAnimationEvent -= HandleAnimationEvent;
        }

        private void Update()
        {
            HandleInput();

            if (_stateMachine != null)
            {
                _stateMachine.Update(Time.deltaTime);
            }
        }

        // -------------------- Input --------------------

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                TryStartAbility(_abilities[0]);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                TryStartAbility(_abilities[1]);

            // Отмена способности — только если сейчас TargetingState и стратегия разрешает
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1))
                _stateMachine?.TryCancel();
        }

        // -------------------- Lifecycle --------------------

        private void TryStartAbility(AbilityRuntime ability)
        {
            if (ability == null) return;
            if (IsCasting) return;
            if (GetCooldown(ability) > 0f) return;

            StartAbility(ability);
        }

        private void StartAbility(AbilityRuntime ability)
        {
            _currentAbility = ability;
            var _ctx = new AbilityContext
            {
                Caster = gameObject,
                Animator = Animator,
                CastOrigin = CastOrigin,
                Ability = _currentAbility,
            };

            _stateMachine = new AbilityStateMachine(_ctx);
            _ctx.OwnerMachine = _stateMachine;

            // Свежие runtime-стратегии на каждую активацию
            _stateMachine.ChangeState(new WarmupState(_currentAbility.Data.Warmup.CreateRuntime()));
        }

        public bool TryInterrupt()
        {
            if (_stateMachine == null || _stateMachine.IsFinished) return false;

            return true;
        }

        // -------------------- AnimationData Events --------------------

        private void HandleAnimationEvent(string eventName)
        {
            if (_stateMachine == null || _stateMachine.IsFinished) return;
            if (_stateMachine.Current?.Strategy is IAnimationEventSink sink)
                sink.OnAnimationEvent(eventName);
        }

        // -------------------- Cooldowns --------------------

        private float GetCooldown(AbilityRuntime ability) => ability.CooldownTimer.CurrentTime;
    }
}