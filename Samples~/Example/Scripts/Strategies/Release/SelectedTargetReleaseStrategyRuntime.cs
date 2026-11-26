using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Applies a release to components implementing IAbilityTargetEffectReceiver
    /// on the selected target (and optionally its parent hierarchy).
    /// </summary>
    public class SelectedTargetReleaseStrategyRuntime : ReleaseStrategyRuntime
    {
        private readonly SelectedTargetReleaseStrategyConfig _config;

        /// <summary>Creates a release runtime configured to invoke target effect receivers.</summary>
        public SelectedTargetReleaseStrategyRuntime(SelectedTargetReleaseStrategyConfig config) : base(config)
        {
            _config = config;
        }

        /// <summary>Runs the base release setup and applies the ability to receivers found on the selected target.</summary>
        public override void OnEnter(AbilityContext ctx)
        {
            base.OnEnter(ctx);

            if (ctx.Target == null)
                return;

            MonoBehaviour[] receivers = _config.SearchParents
                ? ctx.Target.GetComponentsInParent<MonoBehaviour>(true)
                : ctx.Target.GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour component in receivers)
            {
                if (component is IAbilityTargetEffectReceiver receiver)
                    receiver.ApplyAbility(ctx);
            }
        }
    }
}
