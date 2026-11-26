using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class TargetingStrategyRuntime : IAbilityStateStrategy
    {
        protected readonly TargetingStrategyConfig _cfg;
        private float _elapsed;

        public TargetingStrategyRuntime(TargetingStrategyConfig cfg) => _cfg = cfg;

        public virtual void OnEnter(AbilityContext ctx)
        {
            _elapsed = 0f;
            ctx.Target = null;
            ctx.TargetPoint = Vector3.zero;

            if (_cfg.VFX != null)
                Object.Instantiate(_cfg.VFX, ctx.Caster.transform);
            if (_cfg.SFX != null)
                AudioSource.PlayClipAtPoint(_cfg.SFX, ctx.Caster.transform.position);

            if (ctx.Animator != null && !string.IsNullOrEmpty(_cfg.AnimationData.AnimationName))
            {
                ctx.Animator.SetBool("IsTargeting", true);
                ctx.Animator.CrossFadeInFixedTime(_cfg.AnimationData.AnimationName, _cfg.AnimationData.Crossfade);
            }
        }

        public virtual void OnUpdate(AbilityContext ctx)
        {
            _elapsed += Time.deltaTime;
            UpdateTargeting(ctx);
            if (_elapsed >= _cfg.MaxTargetingTime)
                ctx.OwnerMachine.ChangeState(new CastingState(ctx.Ability.Data.Casting.CreateRuntime()));
        }

        protected virtual void UpdateTargeting(AbilityContext ctx) { }

        public virtual void OnExit(AbilityContext ctx)
        {
            if (ctx.Animator != null)
                ctx.Animator.SetBool("IsTargeting", false);
        }

        public virtual bool CanCancel(AbilityContext ctx)
            => _cfg.AllowCancel && _elapsed >= _cfg.MinTimeBeforeCancel;

        public bool CanBeInterrupted(AbilityContext ctx) => _cfg.IsInterruptible;
    }
}
