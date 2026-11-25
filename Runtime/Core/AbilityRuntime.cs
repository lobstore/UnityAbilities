using LOGIYGames.Timers;

namespace LOGIYGames.Abilities
{
    public class AbilityRuntime
    {
        public AbilityData Data { get; private set; }
        public CountdownTimer CooldownTimer { get; private set; }
        public AbilityRuntime(AbilityData Data)
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