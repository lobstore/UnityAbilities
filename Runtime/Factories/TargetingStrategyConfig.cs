using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Strategies/Targeting/Basic")]
    public class TargetingStrategyConfig : AbilityStateStrategyConfig
    {
        public float MaxTargetingTime = 2f;

        // AllowCancel and MinTimeBeforeCancel are inherited from
        // AbilityStateStrategyConfig so every state uses the same policy.

        public override IAbilityStateStrategy CreateRuntime() => new TargetingStrategyRuntime(this);
    }
}
