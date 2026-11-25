namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Runtime behavior and policy contract for one phase of an ability.
    /// Implementations are created per activation by a strategy config asset.
    /// Store mutable per-activation data in the strategy instance, not in the config asset.
    /// </summary>
    public interface IAbilityStateStrategy
    {
        /// <summary>
        /// Called once when this state becomes active. Reset timers and transient flags here.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        void OnEnter(AbilityContext ctx);

        /// <summary>
        /// Called on each state-machine update while this state is active.
        /// Request transitions through <see cref="AbilityContext.OwnerMachine"/>.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        void OnUpdate(AbilityContext ctx);

        /// <summary>
        /// Called once when this state is left, whether by a normal transition, cancellation,
        /// interruption, or ability completion. Release resources owned by this state here.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        void OnExit(AbilityContext ctx);

        /// <summary>
        /// Determines whether an explicit cancellation request is currently allowed.
        /// The method should be side-effect free.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        /// <returns><see langword="true"/> when cancellation is allowed; otherwise, <see langword="false"/>.</returns>
        bool CanCancel(AbilityContext ctx);

        /// <summary>
        /// Determines whether another ability may replace the current ability in this state.
        /// This policy is independent from <see cref="CanCancel"/> and should be side-effect free.
        /// </summary>
        /// <param name="ctx">Shared context for the current ability activation.</param>
        /// <returns><see langword="true"/> when interruption is allowed; otherwise, <see langword="false"/>.</returns>
        bool CanBeInterrupted(AbilityContext ctx);
    }
}
