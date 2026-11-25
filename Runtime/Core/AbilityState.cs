namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Represents one phase in an ability's lifecycle and delegates its behavior
    /// and policy decisions to the configured runtime strategy.
    /// </summary>
    public abstract class AbilityState
    {
        /// <summary>
        /// Gets the runtime strategy that implements this state's behavior.
        /// </summary>
        public IAbilityStateStrategy Strategy { get; protected set; }

        /// <summary>
        /// Enters this state and invokes the strategy's <see cref="IAbilityStateStrategy.OnEnter"/> callback.
        /// </summary>
        /// <param name="ctx">Shared context for the currently executing ability.</param>
        public virtual void Enter(AbilityContext ctx) => Strategy?.OnEnter(ctx);

        /// <summary>
        /// Updates this state by invoking the strategy's <see cref="IAbilityStateStrategy.OnUpdate"/> callback.
        /// </summary>
        /// <param name="ctx">Shared context for the currently executing ability.</param>
        public virtual void Update(AbilityContext ctx) => Strategy?.OnUpdate(ctx);

        /// <summary>
        /// Exits this state and invokes the strategy's <see cref="IAbilityStateStrategy.OnExit"/> callback.
        /// </summary>
        /// <param name="ctx">Shared context for the currently executing ability.</param>
        public virtual void Exit(AbilityContext ctx) => Strategy?.OnExit(ctx);

        /// <summary>
        /// Determines whether an explicit cancellation request is currently allowed.
        /// </summary>
        /// <param name="ctx">Shared context for the currently executing ability.</param>
        /// <returns><see langword="true"/> when the strategy allows cancellation; otherwise, <see langword="false"/>.</returns>
        public virtual bool CanCancel(AbilityContext ctx) => Strategy?.CanCancel(ctx) ?? false;

        /// <summary>
        /// Determines whether another ability is allowed to interrupt the current ability in this state.
        /// </summary>
        /// <param name="ctx">Shared context for the currently executing ability.</param>
        /// <returns><see langword="true"/> when interruption is allowed; otherwise, <see langword="false"/>.</returns>
        public virtual bool CanBeInterrupted(AbilityContext ctx)
            => Strategy?.CanBeInterrupted(ctx) ?? false;
    }

    /// <summary>
    /// Ability phase used for startup animations, effects, and preparation.
    /// </summary>
    public class WarmupState : AbilityState
    {
        /// <summary>
        /// Creates a Warmup state backed by the supplied runtime strategy.
        /// </summary>
        /// <param name="strategy">Runtime strategy that implements Warmup behavior.</param>
        public WarmupState(IAbilityStateStrategy strategy) => Strategy = strategy;
    }

    /// <summary>
    /// Ability phase used to acquire or calculate a target and target point.
    /// </summary>
    public class TargetingState : AbilityState
    {
        /// <summary>
        /// Creates a Targeting state backed by the supplied runtime strategy.
        /// </summary>
        /// <param name="strategy">Runtime strategy that implements Targeting behavior.</param>
        public TargetingState(IAbilityStateStrategy strategy) => Strategy = strategy;
    }

    /// <summary>
    /// Ability phase used for cast-time behavior before the ability is released.
    /// </summary>
    public class CastingState : AbilityState
    {
        /// <summary>
        /// Creates a Casting state backed by the supplied runtime strategy.
        /// </summary>
        /// <param name="strategy">Runtime strategy that implements Casting behavior.</param>
        public CastingState(IAbilityStateStrategy strategy) => Strategy = strategy;
    }

    /// <summary>
    /// Ability phase that performs the ability's final action, such as spawning a projectile or applying a hit.
    /// </summary>
    public class ReleaseState : AbilityState
    {
        /// <summary>
        /// Creates a Release state backed by the supplied runtime strategy.
        /// </summary>
        /// <param name="strategy">Runtime strategy that implements Release behavior.</param>
        public ReleaseState(IAbilityStateStrategy strategy) => Strategy = strategy;
    }
}
