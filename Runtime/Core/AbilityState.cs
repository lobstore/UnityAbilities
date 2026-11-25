namespace LOGIYGames.Abilities
{
    public abstract class AbilityState
    {
        public IAbilityStateStrategy Strategy { get; protected set; }
        public virtual void Enter(AbilityContext ctx) => Strategy?.OnEnter(ctx);
        public virtual void Update(AbilityContext ctx) => Strategy?.OnUpdate(ctx);
        public virtual void Exit(AbilityContext ctx) => Strategy?.OnExit(ctx);
        public virtual bool CanCancel(AbilityContext ctx) => Strategy?.CanCancel(ctx) ?? false;
    }
    public class WarmupState : AbilityState
    {
        public WarmupState(IAbilityStateStrategy strategy) => Strategy = strategy;
    }

    public class TargetingState : AbilityState
    {
        public TargetingState(IAbilityStateStrategy strategy) => Strategy = strategy;
    }

    public class CastingState : AbilityState
    {
        public CastingState(IAbilityStateStrategy strategy) => Strategy = strategy;
    }

    public class ReleaseState : AbilityState
    {
        public ReleaseState(IAbilityStateStrategy strategy) => Strategy = strategy;
    }
}
