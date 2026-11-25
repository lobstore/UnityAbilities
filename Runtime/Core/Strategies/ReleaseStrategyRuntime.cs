using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class ReleaseStrategyRuntime : IAbilityStateStrategy, IAnimationEventSink
    {
        private readonly ReleaseStrategyConfig _cfg;
        private float _elapsed;
        private bool _animEvent;
        public ReleaseStrategyRuntime(ReleaseStrategyConfig cfg)
        {
            _cfg = cfg;
        }

        public bool CanBeInterrupted(AbilityContext ctx)
        {
            return _cfg.IsInterruptible;
        }

        public virtual bool CanCancel(AbilityContext ctx)
        {
            return false;
        }

        public void OnAnimationEvent(string eventName)
        {
            if (eventName == _cfg.AnimationEventName)
            {
                _animEvent = true;
            }
        }

        public virtual void OnEnter(AbilityContext ctx)
        {
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
            if (ctx.Animator != null) ctx.Animator.SetBool("IsReleasing", false);
            ctx.Ability.SetOnCooldown();
        }

        public virtual void OnUpdate(AbilityContext ctx)
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= _cfg.StateDuration || (_cfg.ExitStateOnAnimationEvent && _animEvent))
            {
                ctx.OwnerMachine.Finish();
            }
        }
    }
}
