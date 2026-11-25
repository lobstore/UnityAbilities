using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Default Casting phase implementation. Waits for its configured duration or
    /// animation event, then transitions to the Release phase.
    /// </summary>
    public class CastingStrategyRuntime : IAbilityStateStrategy, IAnimationEventSink
    {
        private readonly CastingStrategyConfig _cfg;
        private float _elapsed;
        private bool _animEvent;

        /// <summary>
        /// Creates a Casting runtime using the supplied authoring configuration.
        /// </summary>
        /// <param name="cfg">Casting settings asset for this ability.</param>
        public CastingStrategyRuntime(CastingStrategyConfig cfg) => _cfg = cfg;

        /// <summary>
        /// Resets the cast timer and animation-event flag, then starts configured effects and animation.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        public void OnEnter(AbilityContext ctx)
        {
            _elapsed = 0f;
            _animEvent = false;

            if (_cfg.StateDuration == 0)
                return;

            if (_cfg.VFX != null)
                Object.Instantiate(_cfg.VFX, ctx.Caster.transform);
            if (_cfg.SFX != null)
                AudioSource.PlayClipAtPoint(_cfg.SFX, ctx.Caster.transform.position);

            if (ctx.Animator != null)
            {
                ctx.Animator.SetBool("IsCasting", true);
                ctx.Animator.CrossFadeInFixedTime(_cfg.AnimationData.AnimationName, _cfg.AnimationData.Crossfade);
            }
        }

        /// <summary>
        /// Advances the cast phase and transitions to Release when the duration or configured
        /// animation-event condition is met.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        public void OnUpdate(AbilityContext ctx)
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= _cfg.StateDuration || (_cfg.ExitStateOnAnimationEvent && _animEvent))
                ctx.OwnerMachine.ChangeState(new ReleaseState(ctx.Ability.Data.Release.CreateRuntime()));
        }

        /// <summary>
        /// Restores the Casting Animator flag when this phase exits.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        public void OnExit(AbilityContext ctx)
        {
            if (ctx.Animator != null)
                ctx.Animator.SetBool("IsCasting", false);
        }

        /// <summary>
        /// Checks whether cancellation is enabled and the minimum time in Casting has elapsed.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        /// <returns><see langword="true"/> when cancellation is allowed.</returns>
        public bool CanCancel(AbilityContext ctx)
            => _cfg.AllowCancel && _elapsed >= _cfg.MinTimeBeforeCancel;

        /// <summary>
        /// Receives Animator events and records the configured Casting exit event.
        /// </summary>
        /// <param name="eventName">Event name emitted by the Animator clip.</param>
        public void OnAnimationEvent(string eventName)
        {
            if (eventName == _cfg.AnimationEventName)
                _animEvent = true;
        }

        /// <summary>
        /// Returns whether the Casting config allows this phase to be interrupted by another ability.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        /// <returns><see langword="true"/> when interruption is allowed.</returns>
        public bool CanBeInterrupted(AbilityContext ctx) => _cfg.IsInterruptible;
    }
}
