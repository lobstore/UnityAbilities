using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// ScriptableObject settings for abilities that always target their caster.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Strategies/Targeting/Self")]
    public class SelfTargetingStrategyConfig : TargetingStrategyConfig
    {
        /// <summary>Creates a new self-targeting runtime for one ability activation.</summary>
        /// <returns>A fresh <see cref="SelfTargetingRuntime"/> instance.</returns>
        public override IAbilityStateStrategy CreateRuntime() => new SelfTargetingRuntime(this);
    }
}
