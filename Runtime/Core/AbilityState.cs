namespace LOGIYGames.Abilities
{
    public class AbilityState
    {
        public AbilityStrategyRuntime Strategy { get; protected set; }
        public AbilityContext Context => Strategy != null ? Strategy.Context : null;
        public AbilityState(AbilityStrategyRuntime strategy) { Strategy = strategy; }
        public virtual void Enter() => Strategy?.OnEnter();
        public virtual void Update() => Strategy?.OnUpdate();
        public virtual void Exit() => Strategy?.OnExit();
        public virtual bool CanBeCancelled() => Strategy?.CanBeCancelled() ?? false;
        public virtual bool CanBeInterrupted() => Strategy?.CanBeInterrupted() ?? false;
    }
}