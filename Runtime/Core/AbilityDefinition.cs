using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Ability Data")]
    public class AbilityDefinition : ScriptableObject
    {
        public string AbilityName;
        public GameObject Prefab;
        public Sprite Icon;
        public string Description;
        [Min(0)] public float CooldownDuration = 0;
        public WarmupStrategyConfig Warmup;
        public TargetingStrategyConfig Targeting;
        public CastingStrategyConfig Casting;
        public ReleaseStrategyConfig Release;
        public ExitingStrategyConfig Exiting;
    }
}
