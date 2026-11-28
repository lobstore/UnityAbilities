namespace LOGIYGames.Abilities
{
    public class AbilityState
    {
        public AbilityStrategyRuntime Strategy { get; protected set; }

        public AbilityState(AbilityStrategyRuntime strategy)
        {
            Strategy = strategy;
        }

        public virtual void Enter() => Strategy?.OnEnter();
        public virtual void Update() => Strategy?.OnUpdate();
        public virtual void Exit() => Strategy?.OnExit();

        /// <summary>
        /// Returns whether cancellation is currently allowed by this state's strategy.
        /// </summary>
        public virtual bool CanBeCancelled() => Strategy?.CanBeCancelled() ?? false;

        /// <summary>
        /// Returns whether another ability is allowed to interrupt this state.
        /// </summary>
        public virtual bool CanBeInterrupted() => Strategy?.CanBeInterrupted() ?? false;
    }

}
