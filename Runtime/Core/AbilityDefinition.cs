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
        [SerializeReference] public WarmupStrategyFactory Warmup = new WarmupStrategyFactory();
        [SerializeReference] public TargetingStrategyFactory Targeting = new TargetingStrategyFactory();
        [SerializeReference] public CastingStrategyFactory Casting = new CastingStrategyFactory();
        [SerializeReference] public ReleaseStrategyFactory Release = new ReleaseStrategyFactory();
        [SerializeReference] public ExitingStrategyFactory Exiting = new ExitingStrategyFactory();
    }
}
