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

    //public class WarmupState : AbilityState
    //{
    //    public WarmupState(IAbilityStateStrategy strategy) => Strategy = strategy;
    //}

    //public class TargetingState : AbilityState
    //{
    //    public TargetingState(IAbilityStateStrategy strategy) => Strategy = strategy;
    //}

    //public class CastingState : AbilityState
    //{
    //    public CastingState(IAbilityStateStrategy strategy) => Strategy = strategy;
    //}

    //public class ReleaseState : AbilityState
    //{
    //    public ReleaseState(IAbilityStateStrategy strategy) => Strategy = strategy;
    //}
    //public class ExitingState : AbilityState
    //{
    //    public ExitingState(IAbilityStateStrategy strategy) => Strategy = strategy;
    //}
}
