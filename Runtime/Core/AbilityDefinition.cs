using System.Collections.Generic;
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

        [Header("Ability Modules")]
        [SerializeReference] public List<AbilityModuleFactory> Modules = new();

        [Header("Ability Flow")]
        [SerializeReference] public List<AbilityFlowModuleFactory> FlowModules = new();

        [Header("Progression")]
        [SerializeReference] public AbilityProgressionDefinition Progression = new();

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