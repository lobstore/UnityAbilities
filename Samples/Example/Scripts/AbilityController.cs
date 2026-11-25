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
        public AbilityState CurrentState => _stateMachine?.Current;
        public AbilityRuntime CurrentAbility => _currentAbility;
        public bool CanCancelCurrentAbility => _stateMachine?.CanCancel ?? false;
        public bool CanInterruptCurrentAbility => _stateMachine?.CanBeInterrupted ?? false;

        private readonly List<AbilityRuntime> _abilities = new();
        public IReadOnlyList<AbilityRuntime> Abilities => _abilities;

        private AbilityRuntime _currentAbility;

        private void Awake()
        {
            if (PrimaryAbility != null)
                _abilities.Add(new AbilityRuntime(PrimaryAbility));
            if (SecondaryAbility != null)
                _abilities.Add(new AbilityRuntime(SecondaryAbility));

            if (Animator == null)
                Animator = GetComponent<Animator>();
            if (CastOrigin == null)
                CastOrigin = transform;

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
                _stateMachine.Update(Time.deltaTime);
        }

        // -------------------- Input --------------------

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && _abilities.Count > 0)
                TryActivateAbility(_abilities[0]);
            else if (Input.GetKeyDown(KeyCode.Alpha2) && _abilities.Count > 1)
                TryActivateAbility(_abilities[1]);

            // Escape and the right mouse button request a policy-checked cancellation.
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1))
                TryCancelCurrentAbility();
        }

        // -------------------- Public API --------------------

        /// <summary>
        /// Starts an ability. When another ability is active, set interruptCurrent
        /// to true to replace it if its current state allows interruption.
        /// </summary>
        public bool TryActivateAbility(AbilityRuntime ability, bool interruptCurrent = false)
        {
            if (ability == null || ability.Data == null)
                return false;

            if (GetCooldown(ability) > 0f)
                return false;

            if (IsCasting)
            {
                if (!interruptCurrent || !TryInterrupt())
                    return false;
            }

            StartAbility(ability);
            return true;
        }

        /// <summary>
        /// Requests cancellation according to the current state's AllowCancel settings.
        /// </summary>
        public bool TryCancelCurrentAbility()
            => _stateMachine != null && _stateMachine.TryCancel();

        /// <summary>
        /// Requests interruption according to the current state's IsInterruptible setting.
        /// </summary>
        public bool TryInterrupt()
            => _stateMachine != null && _stateMachine.TryInterrupt();

        // -------------------- Lifecycle --------------------

        private void StartAbility(AbilityRuntime ability)
        {
            _currentAbility = ability;
            var ctx = new AbilityContext
            {
                Caster = gameObject,
                Animator = Animator,
                CastOrigin = CastOrigin,
                Ability = _currentAbility,
            };

            _stateMachine = new AbilityStateMachine(ctx);
            ctx.OwnerMachine = _stateMachine;

            // Create fresh runtime strategies for every activation.
            _stateMachine.ChangeState(new WarmupState(_currentAbility.Data.Warmup.CreateRuntime()));
        }

        // -------------------- Animation Events --------------------

        private void HandleAnimationEvent(string eventName)
        {
            if (_stateMachine == null || _stateMachine.IsFinished)
                return;

            if (_stateMachine.Current?.Strategy is IAnimationEventSink sink)
                sink.OnAnimationEvent(eventName);
        }

        // -------------------- Cooldowns --------------------

        private float GetCooldown(AbilityRuntime ability) => ability.CooldownTimer.CurrentTime;
    }
}
