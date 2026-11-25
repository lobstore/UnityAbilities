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
                ctx.OwnerMachine.Finish();
        }
    }
}
