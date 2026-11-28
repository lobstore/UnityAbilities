using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class AbilityTargetData
    {
        public List<GameObject> Targets;
        public Vector3 Position;
        public Vector3 Direction;
    }

    public class AbilityContext
    {
        public AbilityStateMachine OwnerMachine;
        public Ability Ability;
        public AbilityTargetData TargetingData;
        public GameObject Caster;
        public int CurrentIteration;
        public Transform CastOrigin;
        public int AbilityLevel = 1;
        public AbilityProgressionState ProgressionState;
        public AbilityParameters Parameters = new();
        public AbilityRuntimeServices Services { get; private set; }

        public void InitializeServices()
        {
            if (Services != null || Ability == null || Ability.Data == null) return;
            if (ProgressionState == null) ProgressionState = Ability.ProgressionState;
            if (ProgressionState != null) AbilityLevel = ProgressionState.Level;
            AbilityLevel = Mathf.Max(1, AbilityLevel);
            Parameters ??= new AbilityParameters();
            ApplyProgressionParameters();
            Services = new AbilityRuntimeServices(this);
            Services.Initialize();
        }

        public AbilityState CreateState(AbilityPhase phase)
        {
            if (Ability == null || Ability.Data == null) return null;
            AbilityStrategyRuntime runtime = phase switch
            {
                AbilityPhase.Warmup => Ability.Data.Warmup.CreateRuntime(this),
                AbilityPhase.Targeting => Ability.Data.Targeting.CreateRuntime(this),
                AbilityPhase.Casting => Ability.Data.Casting.CreateRuntime(this),
                AbilityPhase.Release => Ability.Data.Release.CreateRuntime(this),
                AbilityPhase.Exiting => Ability.Data.Exiting.CreateRuntime(this),
                _ => null
            };
            return runtime != null ? new AbilityState(runtime) : null;
        }

        public void ApplyProgressionParameters()
        {
            if (Parameters == null) Parameters = new AbilityParameters();
            float damageMultiplier = 1f;
            float cooldownMultiplier = 1f;
            AbilityProgressionDefinition progression = Ability?.Data?.Progression;
            if (progression != null)
            {
                for (int level = 1; level <= AbilityLevel; level++)
                {
                    AbilityLevelDefinition levelDefinition = progression.GetLevel(level);
                    if (levelDefinition == null) continue;
                    damageMultiplier *= Mathf.Max(0f, levelDefinition.DamageMultiplier);
                    cooldownMultiplier *= Mathf.Max(0f, levelDefinition.CooldownMultiplier);
                }
            }
            Parameters.SetFloat("DamageMultiplier", damageMultiplier);
            Parameters.SetFloat("CooldownMultiplier", cooldownMultiplier);
        }
    }
}