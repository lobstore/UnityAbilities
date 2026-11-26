using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class WarmupStrategyRuntime : IAbilityStateStrategy, IAnimationEventSink
    {
        private readonly WarmupStrategyConfig _cfg;
        private readonly AbilityCinematicCameraRuntime _cameraRuntime = new();
        private float _elapsed;
        private bool _animEvent;

        public WarmupStrategyRuntime(WarmupStrategyConfig cfg) => _cfg = cfg;

        public virtual void OnEnter(AbilityContext ctx)
        {
            _elapsed = 0f;
            _animEvent = false;

            if (_cfg.VFX != null)
                UnityEngine.Object.Instantiate(_cfg.VFX, ctx.Caster.transform);
            if (_cfg.SFX != null)
                AudioSource.PlayClipAtPoint(_cfg.SFX, ctx.Caster.transform.position);
            if (ctx.Animator != null && !string.IsNullOrEmpty(_cfg.AnimationData.AnimationName))
            {
                ctx.Animator.SetBool("IsWarmuping", true);
                ctx.Animator.CrossFadeInFixedTime(_cfg.AnimationData.AnimationName, _cfg.AnimationData.Crossfade);
            }

            _cameraRuntime.OnEnter(ctx, _cfg);
        }

        public virtual void OnUpdate(AbilityContext ctx)
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= _cfg.StateDuration || (_cfg.ExitStateOnAnimationEvent && _animEvent))
                ctx.OwnerMachine.ChangeState(new TargetingState(ctx.Ability.Data.Targeting.CreateRuntime()));
        }

        public virtual void OnExit(AbilityContext ctx)
        {
            try
            {
                _cameraRuntime.OnExit();
            }
            finally
            {
                if (ctx.Animator != null)
                    ctx.Animator.SetBool("IsWarmuping", false);
            }
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