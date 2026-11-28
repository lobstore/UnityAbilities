using LOGIYGames.Timers;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class Ability
    {
        public AbilityDefinition Data { get; private set; }
        public CountdownTimer CooldownTimer { get; private set; }
        public AbilityProgressionState ProgressionState { get; } = new();

        public Ability(AbilityDefinition Data)
        {
            this.Data = Data;
            CooldownTimer = new CountdownTimer();
        }

        public void SetOnCooldown() => SetOnCooldown(ProgressionState.Level);

        public void SetOnCooldown(int abilityLevel)
        {
            float multiplier = 1f;
            AbilityProgressionDefinition progression = Data != null ? Data.Progression : null;
            if (progression != null)
            {
                for (int level = 1; level <= Mathf.Max(1, abilityLevel); level++)
                {
                    AbilityLevelDefinition levelDefinition = progression.GetLevel(level);
                    if (levelDefinition != null)
                        multiplier *= Mathf.Max(0f, levelDefinition.CooldownMultiplier);
                }
            }
            CooldownTimer.Reset(Data.CooldownDuration * multiplier);
            CooldownTimer.Start();
        }
    }
}