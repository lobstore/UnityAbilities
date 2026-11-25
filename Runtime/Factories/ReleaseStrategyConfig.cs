using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Strategies/Release")]
    public class ReleaseStrategyConfig : AbilityStateStrategyConfig
    {
        public override IAbilityStateStrategy CreateRuntime()
        {
            return new ReleaseStrategyRuntime(this);
        }
    }
}
