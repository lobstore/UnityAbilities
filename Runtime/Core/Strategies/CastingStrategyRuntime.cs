using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class CastingStrategyRuntime : IAbilityStateStrategy, IAnimationEventSink
    {
        private readonly CastingStrategyConfig _cfg;
        private float _elapsed;
        private bool _animEvent;

        public CastingStrategyRuntime(CastingStrategyConfig cfg) => _cfg = cfg;

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

        public void OnUpdate(AbilityContext ctx)
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= _cfg.StateDuration || (_cfg.ExitStateOnAnimationEvent && _animEvent))
                ctx.OwnerMachine.ChangeState(new ReleaseState(ctx.Ability.Data.Release.CreateRuntime()));
        }

        public void OnExit(AbilityContext ctx)
        {
            if (ctx.Animator != null)
                ctx.Animator.SetBool("IsCasting", false);
        }

        public bool CanCancel(AbilityContext ctx)
            => _cfg.AllowCancel && _elapsed >= _cfg.MinTimeBeforeCancel;

        public void OnAnimationEvent(string eventName)
        {
            if (eventName == _cfg.AnimationEventName)
                _animEvent = true;
        }

        public bool CanBeInterrupted(AbilityContext ctx) => _cfg.IsInterruptible;
    }
}
