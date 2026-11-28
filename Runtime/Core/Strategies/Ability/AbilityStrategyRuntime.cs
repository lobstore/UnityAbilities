using LOGIYGames.Timers;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public abstract class AbilityStrategyRuntime
    {
        private readonly AbilityStateStrategyFactory _cfg;
        protected bool _animExitEventTriggered;
        protected AbilityContext _ctx;
        protected CountdownTimer DurationTimer = null;
        private int _stateId;
        private bool _stateEntered;

        public AbilityContext Context => _ctx;
        protected virtual AbilityPhase Phase => AbilityPhase.None;

        protected AbilityStrategyRuntime(AbilityStateStrategyFactory cfg, AbilityContext ctx)
        {
            _cfg = cfg; _ctx = ctx; DurationTimer = new CountdownTimer(_cfg.StateDuration);
        }

        public virtual bool CanBeInterrupted() => _cfg.IsInterruptible;
        public virtual bool CanBeCancelled() => _cfg.AllowCancel;

        public virtual void OnEnter()
        {
            _ctx.InitializeServices();
            _stateId = _ctx.Services != null ? _ctx.Services.BeginState(Phase, _cfg) : 0;
            _stateEntered = true;
            DurationTimer.Reset();
            DurationTimer.Start();
            _animExitEventTriggered = false;

            if (!string.IsNullOrEmpty(_cfg.AnimationData.AnimationName) &&
                _ctx.Caster != null && _ctx.Caster.TryGetComponent(out Animator animator))
            {
                animator.applyRootMotion = _cfg.AnimationData.UseRootMotion;
                animator.SetFloat("MotionSpeed", _cfg.AnimationData.MotionSpeed);
                animator.CrossFadeInFixedTime(_cfg.AnimationData.AnimationName,
                    _cfg.AnimationData.Crossfade, (int)_cfg.AnimationData.layerIndex);
            }
        }

        public virtual void OnUpdate()
        {
            _ctx.Services?.Tick(Time.deltaTime);
            if (StateExitCondition()) StateTransitionRules();
        }

        protected virtual bool StateExitCondition()
        {
            switch (_cfg.ExitOn)
            {
                case AbilityStateExitType.Timer: return DurationTimer.IsFinished;
                case AbilityStateExitType.AnimationEvent: return _animExitEventTriggered;
                default: return false;
            }
        }

        protected abstract void StateTransitionRules();

        public virtual void OnExit()
        {
            DurationTimer.Stop();
            if (!_stateEntered) return;
            _ctx.Services?.EndState(Phase, _stateId);
            _stateEntered = false;
        }

        public void OnAnimationEvent(string eventName)
        {
            _ctx.Services?.PublishAnimationEvent(Phase, eventName);
            if (eventName == _cfg.AnimationExitEventName) _animExitEventTriggered = true;
        }
    }
}