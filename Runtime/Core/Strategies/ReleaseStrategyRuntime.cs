using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class ReleaseStrategyRuntime : IAbilityStateStrategy, IAnimationEventSink
    {
        private readonly ReleaseStrategyConfig _cfg;
        private float _elapsed;
        private bool _animEvent;
        private bool _isRepeatTransition;

        public ReleaseStrategyRuntime(ReleaseStrategyConfig cfg)
        {
            _cfg = cfg;
        }

        public bool CanBeInterrupted(AbilityContext ctx) => _cfg.IsInterruptible;

        public virtual bool CanCancel(AbilityContext ctx)
            => _cfg.AllowCancel && _elapsed >= _cfg.MinTimeBeforeCancel;

        public void OnAnimationEvent(string eventName)
        {
            if (eventName == _cfg.AnimationEventName)
                _animEvent = true;
        }

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

        public virtual void OnExit(AbilityContext ctx)
        {
            if (ctx.Animator != null)
                ctx.Animator.SetBool("IsReleasing", false);

            // A loop transition is part of the same ability activation, not its completion.
            // Do not start cooldown between repeated releases.
            if (_isRepeatTransition)
                return;

            // Cooldown is applied on final completion, cancellation, or interruption
            // after the ability has reached the Release phase.
            ctx.Ability.SetOnCooldown();
        }

        public virtual void OnUpdate(AbilityContext ctx)
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= _cfg.StateDuration || (_cfg.ExitStateOnAnimationEvent && _animEvent))
                OnReleaseCompleted(ctx);
        }

        /// <summary>
        /// Completes the current release iteration. When MaxIterations is greater than one,
        /// starts another targeting phase before completing the ability.
        /// Override this method to customize iteration completion; call the base implementation
        /// on the final iteration to preserve the standard finish behavior.
        /// </summary>
        protected virtual void OnReleaseCompleted(AbilityContext ctx)
        {
            int maxIterations = Mathf.Max(1, _cfg.MaxIterations);
            if (ctx.ReleaseIteration + 1 < maxIterations)
            {
                ctx.ReleaseIteration++;
                // The ability has already released at least once. Keep cooldown active
                // even if it is cancelled during the next Targeting/Casting iteration.
                ctx.Ability.SetOnCooldown();
                _isRepeatTransition = true;
                try
                {
                    ctx.OwnerMachine.ChangeState(
                        new TargetingState(ctx.Ability.Data.Targeting.CreateRuntime()));
                }
                finally
                {
                    _isRepeatTransition = false;
                }

                return;
            }

            ctx.OwnerMachine.Finish();
        }
    }
}
