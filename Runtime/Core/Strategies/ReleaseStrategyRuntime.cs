using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Default Release phase implementation. Runs the final ability phase and
    /// applies the configured cooldown when the phase exits.
    /// </summary>
    public class ReleaseStrategyRuntime : IAbilityStateStrategy, IAnimationEventSink
    {
        private readonly ReleaseStrategyConfig _cfg;
        private float _elapsed;
        private bool _animEvent;

        /// <summary>
        /// Creates a Release runtime using the supplied authoring configuration.
        /// </summary>
        /// <param name="cfg">Release settings asset for this ability.</param>
        public ReleaseStrategyRuntime(ReleaseStrategyConfig cfg)
        {
            _cfg = cfg;
        }

        /// <summary>
        /// Returns whether the Release config permits interruption by another ability.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        /// <returns><see langword="true"/> when interruption is allowed.</returns>
        public bool CanBeInterrupted(AbilityContext ctx) => _cfg.IsInterruptible;

        /// <summary>
        /// Checks whether cancellation is enabled and the minimum time in Release has elapsed.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        /// <returns><see langword="true"/> when cancellation is allowed.</returns>
        public virtual bool CanCancel(AbilityContext ctx)
            => _cfg.AllowCancel && _elapsed >= _cfg.MinTimeBeforeCancel;

        /// <summary>
        /// Receives Animator events and records the configured Release exit event.
        /// </summary>
        /// <param name="eventName">Event name emitted by the Animator clip.</param>
        public void OnAnimationEvent(string eventName)
        {
            if (eventName == _cfg.AnimationEventName)
                _animEvent = true;
        }

        /// <summary>
        /// Resets phase timers/event flags and starts configured effects and animation.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        public virtual void OnEnter(AbilityContext ctx)
        {
            _elapsed = 0f;
            _animEvent = false;

            if (_cfg.VFX != null)
                Object.Instantiate(_cfg.VFX, ctx.Caster.transform);
            if (_cfg.SFX != null)
                AudioSource.PlayClipAtPoint(_cfg.SFX, ctx.Caster.transform.position);
            if (ctx.Animator != null)
            {
                ctx.Animator.SetBool("IsReleasing", true);
                ctx.Animator.CrossFadeInFixedTime(_cfg.AnimationData.AnimationName, _cfg.AnimationData.Crossfade);
            }
        }

        /// <summary>
        /// Stops the Release animation flag and starts the ability cooldown.
        /// Called on normal completion, cancellation, or interruption after entering Release.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        public virtual void OnExit(AbilityContext ctx)
        {
            if (ctx.Animator != null)
                ctx.Animator.SetBool("IsReleasing", false);

            // Cooldown is applied when the release phase exits, including cancellation
            // or interruption after the ability has reached this phase.
            ctx.Ability.SetOnCooldown();
        }

        /// <summary>
        /// Advances the Release phase and finishes the ability when its duration or
        /// configured animation-event condition is met.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        public virtual void OnUpdate(AbilityContext ctx)
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= _cfg.StateDuration || (_cfg.ExitStateOnAnimationEvent && _animEvent))
                ctx.OwnerMachine.Finish();
        }
    }
}
