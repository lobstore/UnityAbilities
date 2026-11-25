namespace LOGIYGames.Abilities
{
    public interface IAbilityStateStrategy
    {
        void OnEnter(AbilityContext ctx);
        void OnUpdate(AbilityContext ctx);
        void OnExit(AbilityContext ctx);
        bool CanCancel(AbilityContext ctx);
        bool CanBeInterrupted(AbilityContext ctx);
    }

}
