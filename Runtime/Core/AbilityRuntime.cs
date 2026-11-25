using LOGIYGames.Timers;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Runtime wrapper for an ability asset, including its cooldown timer.
    /// Create one runtime per ability slot or owner; runtime state is not stored in the shared ScriptableObject.
    /// </summary>
    public class AbilityRuntime
    {
        /// <summary>
        /// Gets the ability configuration asset referenced by this runtime.
        /// </summary>
        public AbilityData Data { get; private set; }

        /// <summary>
        /// Gets the countdown timer tracking the remaining cooldown.
        /// </summary>
        public CountdownTimer CooldownTimer { get; private set; }

        /// <summary>
        /// Creates a runtime instance for the supplied ability asset.
        /// </summary>
        /// <param name="Data">Ability configuration asset to wrap.</param>
        public AbilityRuntime(AbilityData Data)
        {
            this.Data = Data;
            CooldownTimer = new CountdownTimer();
        }

        /// <summary>
        /// Starts or restarts this ability's cooldown using <see cref="AbilityData.CooldownDuration"/>.
        /// </summary>
        public void SetOnCooldown()
        {
            CooldownTimer.Reset(Data.CooldownDuration);
            CooldownTimer.Start();
        }
    }
}