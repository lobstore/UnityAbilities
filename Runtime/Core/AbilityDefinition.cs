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
        [Header("Strategy Factories")]
        [Header("Warmup")]
        [SerializeReference] public WarmupStrategyFactory Warmup = new WarmupStrategyFactory();
        [Header("Targeting")]
        [SerializeReference] public TargetingStrategyFactory Targeting = new TargetingStrategyFactory();
        [Header("Casting")]
        [SerializeReference] public CastingStrategyFactory Casting = new CastingStrategyFactory();
        [Header("Release")]
        [SerializeReference] public ReleaseStrategyFactory Release = new ReleaseStrategyFactory();
        [Header("Exiting")]
        [SerializeReference] public ExitingStrategyFactory Exiting = new ExitingStrategyFactory();
    }
}
