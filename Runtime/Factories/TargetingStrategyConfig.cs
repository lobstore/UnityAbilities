using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Strategies/Targeting/Basic")]
    public class TargetingStrategyConfig : AbilityStateStrategyConfig
    {
        public float MaxTargetingTime = 2f;
        public bool AllowCancel = true;
        public float MinTimeBeforeCancel = 0.3f;

        public override IAbilityStateStrategy CreateRuntime() => new TargetingStrategyRuntime(this);
    }
}
