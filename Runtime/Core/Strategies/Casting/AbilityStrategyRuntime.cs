using LOGIYGames.Timers;
using UnityEngine;

namespace LOGIYGames.Abilities
{

    public abstract class AbilityStrategyRuntime
    {
        private readonly AbilityStateStrategyConfig _cfg;
        protected bool _animExitEventTriggered;
        protected AbilityContext _ctx;

        protected CountdownTimer DurationTimer = null;
        protected AbilityStrategyRuntime(AbilityStateStrategyConfig cfg, AbilityContext ctx)
        {
            _cfg = cfg;
            _ctx = ctx;
            DurationTimer = new CountdownTimer(_cfg.StateDuration);
        }
        public virtual bool CanBeInterrupted() => _cfg.IsInterruptible;

        public virtual bool CanBeCancelled() => _cfg.AllowCancel;

        public virtual void OnEnter()
        {
            DurationTimer.Reset();
            DurationTimer.Start();
            _animExitEventTriggered = false;

            if (!string.IsNullOrEmpty( _cfg.AnimationData.AnimationName) )
            {
                if (_ctx.Caster.TryGetComponent(out Animator animator))
                {
                    animator.applyRootMotion = _cfg.AnimationData.UseRootMotion;
                    animator.SetFloat("MotionSpeed", _cfg.AnimationData.MotionSpeed);
                    animator.CrossFadeInFixedTime(_cfg.AnimationData.AnimationName, _cfg.AnimationData.Crossfade, (int)_cfg.AnimationData.layerIndex);
                }
            }
        }
        public virtual void OnUpdate()
        {
            if (StateExitCondition())
                StateTransitionRules();
        }
        protected virtual bool StateExitCondition()
        {
            switch (_cfg.ExitOn)
            {
                case AbilityStateExitType.Timer:
                    return DurationTimer.IsFinished;
                case AbilityStateExitType.AnimationEvent:
                    return _animExitEventTriggered;
                default: return false;
            }
        }
        protected abstract void StateTransitionRules();
        public virtual void OnExit() { DurationTimer.Stop(); }
        public void OnAnimationEvent(string eventName)
        {
            if (eventName == _cfg.AnimationExitEventName)
                _animExitEventTriggered = true;
        }
    }
}
