using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Default Warmup phase implementation. Transitions to Targeting when its
    /// configured duration elapses or its configured animation event is received.
    /// </summary>
    public class WarmupStrategyRuntime : IAbilityStateStrategy, IAnimationEventSink
    {
        private readonly WarmupStrategyConfig _cfg;
        private float _elapsed;
        private bool _animEvent;

        /// <summary>
        /// Creates a Warmup runtime using the supplied authoring configuration.
        /// </summary>
        /// <param name="cfg">Warmup settings asset for this ability.</param>
        public WarmupStrategyRuntime(WarmupStrategyConfig cfg) => _cfg = cfg;

        /// <summary>
        /// Resets phase timers/event flags and starts the configured effects and animation.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        public void OnEnter(AbilityContext ctx)
        {
            _elapsed = 0f;
            _animEvent = false;

            if (_cfg.VFX != null)
                UnityEngine.Object.Instantiate(_cfg.VFX, ctx.Caster.transform);
            if (_cfg.SFX != null)
                AudioSource.PlayClipAtPoint(_cfg.SFX, ctx.Caster.transform.position);
            if (ctx.Animator != null)
            {
                ctx.Animator.SetBool("IsWarmuping", true);
                ctx.Animator.CrossFadeInFixedTime(_cfg.AnimationData.AnimationName, _cfg.AnimationData.Crossfade);
            }
        }

        /// <summary>
        /// Advances Warmup and transitions to Targeting when the configured duration
        /// or animation-event exit condition is met.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        public void OnUpdate(AbilityContext ctx)
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= _cfg.StateDuration || (_cfg.ExitStateOnAnimationEvent && _animEvent))
                ctx.OwnerMachine.ChangeState(new TargetingState(ctx.Ability.Data.Targeting.CreateRuntime()));
        }

        /// <summary>
        /// Restores the Warmup Animator flag when this phase exits.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        public void OnExit(AbilityContext ctx)
        {
            if (ctx.Animator != null)
                ctx.Animator.SetBool("IsWarmuping", false);
        }

        /// <summary>
        /// Checks the configured cancellation permission and minimum time spent in Warmup.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        /// <returns><see langword="true"/> when cancellation is allowed.</returns>
        public bool CanCancel(AbilityContext ctx)
            => _cfg.AllowCancel && _elapsed >= _cfg.MinTimeBeforeCancel;

        /// <summary>
        /// Receives animation event names and records the configured Warmup exit event.
        /// </summary>
        /// <param name="eventName">Event name emitted by the Animator clip.</param>
        public void OnAnimationEvent(string eventName)
        {
            if (eventName == _cfg.AnimationEventName)
                _animEvent = true;
        }

        /// <summary>
        /// Returns whether the Warmup config permits interruption by another ability.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        /// <returns><see langword="true"/> when this phase can be interrupted.</returns>
        public bool CanBeInterrupted(AbilityContext ctx) => _cfg.IsInterruptible;
    }
}
