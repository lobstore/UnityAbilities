namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Compatibility runtime for existing CinematicCastingStrategyConfig assets.
    /// Camera prefab creation and cleanup are handled by CastingStrategyRuntime.
    /// </summary>
    public class CinematicCastingStrategyRuntime : CastingStrategyRuntime
    {
        public CinematicCastingStrategyRuntime(CinematicCastingStrategyConfig config) : base(config) { }
    }
}