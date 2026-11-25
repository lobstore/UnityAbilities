using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Default Targeting phase implementation. Subclasses can override
    /// <see cref="UpdateTargeting"/> to provide custom target acquisition.
    /// </summary>
    public class TargetingStrategyRuntime : IAbilityStateStrategy
    {
        protected readonly TargetingStrategyConfig _cfg;
        private float _elapsed;

        /// <summary>
        /// Creates a Targeting runtime using the supplied authoring configuration.
        /// </summary>
        /// <param name="cfg">Targeting settings asset for this ability.</param>
        public TargetingStrategyRuntime(TargetingStrategyConfig cfg) => _cfg = cfg;

        /// <summary>
        /// Resets targeting state, initializes configured VFX/SFX, and starts the targeting animation.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        public virtual void OnEnter(AbilityContext ctx)
        {
            _elapsed = 0f;
            ctx.Target = null;
            ctx.TargetPoint = Vector3.zero;

            if (_cfg.VFX != null)
                Object.Instantiate(_cfg.VFX, ctx.Caster.transform);
            if (_cfg.SFX != null)
                AudioSource.PlayClipAtPoint(_cfg.SFX, ctx.Caster.transform.position);

            if (ctx.Animator != null)
            {
                ctx.Animator.SetBool("IsTargeting", true);
                ctx.Animator.CrossFadeInFixedTime(_cfg.AnimationData.AnimationName, _cfg.AnimationData.Crossfade);
            }
        }

        /// <summary>
        /// Advances the targeting timer, updates the target, and transitions to Casting
        /// when <see cref="TargetingStrategyConfig.MaxTargetingTime"/> is reached.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        public virtual void OnUpdate(AbilityContext ctx)
        {
            _elapsed += Time.deltaTime;
            UpdateTargeting(ctx);
            if (_elapsed >= _cfg.MaxTargetingTime)
                ctx.OwnerMachine.ChangeState(new CastingState(ctx.Ability.Data.Casting.CreateRuntime()));
        }

        /// <summary>
        /// Hook for subclasses to update <see cref="AbilityContext.Target"/> and
        /// <see cref="AbilityContext.TargetPoint"/> each frame.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        protected virtual void UpdateTargeting(AbilityContext ctx) { }

        /// <summary>
        /// Stops the targeting animation flag when the state exits.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        public virtual void OnExit(AbilityContext ctx)
        {
            if (ctx.Animator != null)
                ctx.Animator.SetBool("IsTargeting", false);
        }

        /// <summary>
        /// Allows cancellation only when configured and the minimum time in Targeting has elapsed.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        /// <returns><see langword="true"/> when cancellation is allowed.</returns>
        public virtual bool CanCancel(AbilityContext ctx)
            => _cfg.AllowCancel && _elapsed >= _cfg.MinTimeBeforeCancel;

        /// <summary>
        /// Returns the Targeting config's interruption setting.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        /// <returns><see langword="true"/> when this phase can be interrupted.</returns>
        public bool CanBeInterrupted(AbilityContext ctx) => _cfg.IsInterruptible;
    }
}
