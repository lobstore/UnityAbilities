using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class WarmupStrategyRuntime : IAbilityStateStrategy, IAnimationEventSink
    {
        private readonly WarmupStrategyConfig _cfg;
        private float _elapsed;
        private bool _animEvent;

        public WarmupStrategyRuntime(WarmupStrategyConfig cfg) => _cfg = cfg;

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

        public void OnUpdate(AbilityContext ctx)
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= _cfg.StateDuration || (_cfg.ExitStateOnAnimationEvent && _animEvent))
                ctx.OwnerMachine.ChangeState(new TargetingState(ctx.Ability.Data.Targeting.CreateRuntime()));
        }

        public void OnExit(AbilityContext ctx)
        {
            if (ctx.Animator != null)
                ctx.Animator.SetBool("IsWarmuping", false);
        }
        public bool CanCancel(AbilityContext ctx) => false;

        public void OnAnimationEvent(string eventName)
        {
            if (eventName == _cfg.AnimationEventName) _animEvent = true;
        }

        public bool CanBeInterrupted(AbilityContext ctx)
        {
            return _cfg.IsInterruptible;
        }
    }
}
