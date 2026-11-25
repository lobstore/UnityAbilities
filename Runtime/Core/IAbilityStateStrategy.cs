namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Runtime behavior and policy decisions for one ability state.
    /// </summary>
    public interface IAbilityStateStrategy
    {
        /// <summary>Called when this state becomes active.</summary>
        void OnEnter(AbilityContext ctx);

        /// <summary>Called while this state is active.</summary>
        void OnUpdate(AbilityContext ctx);

        /// <summary>Called once when this state is left.</summary>
        void OnExit(AbilityContext ctx);

        /// <summary>
        /// Returns whether an explicit cancellation request is currently allowed.
        /// </summary>
        bool CanCancel(AbilityContext ctx);

        /// <summary>
        /// Returns whether another ability may interrupt this state.
        /// </summary>
        bool CanBeInterrupted(AbilityContext ctx);
    }
}
