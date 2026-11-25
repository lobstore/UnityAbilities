using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Ability Data")]
    public class AbilityData : ScriptableObject
    {
        public string AbilityName;
        [Min(0)] public float CooldownDuration = 0;
        public WarmupStrategyConfig Warmup;
        public TargetingStrategyConfig Targeting;
        public CastingStrategyConfig Casting;
        public ReleaseStrategyConfig Release;
    }
}
