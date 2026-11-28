using LOGIYGames.Timers;

namespace LOGIYGames.Abilities
{
    public class Ability
    {
        public AbilityDefinition Data { get; private set; }
        public CountdownTimer CooldownTimer { get; private set; }
        public Ability(AbilityDefinition Data)
        {
            this.Data = Data;
            CooldownTimer = new CountdownTimer();
        }
        public void SetOnCooldown()
        {
            CooldownTimer.Reset(Data.CooldownDuration);
            CooldownTimer.Start();
        }
    }
}