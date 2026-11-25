using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Sample MonoBehaviour that owns an ability loadout, starts ability runtimes,
    /// updates the active state machine, and forwards Animator events to the active strategy.
    /// Games may replace this controller with their own integration.
    /// </summary>
    public class AbilityController : MonoBehaviour
    {
        /// <summary>Ability asset assigned to the first sample input slot.</summary>
        [Header("Loadout")]
        [Tooltip("Клавиша 1 — например, Огненный шар")]
        public AbilityData PrimaryAbility;

        /// <summary>Ability asset assigned to the second sample input slot.</summary>
        [Tooltip("Клавиша 2 — например, Метеор")]
        public AbilityData SecondaryAbility;

        /// <summary>Animator used by ability phase strategies. Resolved from this GameObject when left unset.</summary>
        [Header("References")]
        public Animator Animator;

        /// <summary>Transform used as the origin for released effects and projectiles. Defaults to this transform.</summary>
        public Transform CastOrigin;

        private AbilityStateMachine _stateMachine;
        private AnimationEventReceiver _animReceiver;

        /// <summary>Gets whether an ability state machine is currently active.</summary>
        public bool IsCasting => _stateMachine != null && !_stateMachine.IsFinished;

        /// <summary>Gets the active ability phase, or null when no phase is active.</summary>
        public AbilityState CurrentState => _stateMachine?.Current;

        /// <summary>Gets the active ability runtime, or null when no ability is running.</summary>
        public AbilityRuntime CurrentAbility => IsCasting ? _currentAbility : null;

        /// <summary>Gets whether the active phase currently permits cancellation.</summary>
        public bool CanCancelCurrentAbility => _stateMachine?.CanCancel ?? false;

        /// <summary>Gets whether the active phase currently permits interruption by another ability.</summary>
        public bool CanInterruptCurrentAbility => _stateMachine?.CanBeInterrupted ?? false;

        private readonly List<AbilityRuntime> _abilities = new();

        /// <summary>
        /// Gets the runtime abilities created from the PrimaryAbility and SecondaryAbility assets.
        /// The returned collection is read-only; configure the loadout through the corresponding assets.
        /// </summary>
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
        /// Attempts to start the supplied ability runtime.
        /// If an ability is already active, it is replaced only when
        /// <paramref name="interruptCurrent"/> is true and the active phase allows interruption.
        /// </summary>
        /// <param name="ability">Runtime instance to activate.</param>
        /// <param name="interruptCurrent">Whether the currently active ability may be interrupted to make room for this ability.</param>
        /// <returns><see langword="true"/> when the requested ability starts; otherwise, <see langword="false"/>.</returns>
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
        /// Requests cancellation of the current ability according to the active phase's
        /// <c>AllowCancel</c> and <c>MinTimeBeforeCancel</c> policy.
        /// </summary>
        /// <returns><see langword="true"/> when the ability was cancelled; otherwise, <see langword="false"/>.</returns>
        public bool TryCancelCurrentAbility()
            => _stateMachine != null && _stateMachine.TryCancel();

        /// <summary>
        /// Requests interruption of the current ability according to the active phase's
        /// <c>IsInterruptible</c> policy. This method does not start a replacement ability.
        /// </summary>
        /// <returns><see langword="true"/> when the ability was interrupted; otherwise, <see langword="false"/>.</returns>
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
