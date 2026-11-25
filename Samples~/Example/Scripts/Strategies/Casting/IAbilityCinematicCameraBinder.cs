namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Optional adapter contract for Cinemachine camera prefabs.
    /// Implement it in a component that references the Cinemachine version used by the game,
    /// then add that component to the configured camera prefab.
    /// </summary>
    public interface IAbilityCinematicCameraBinder
    {
        /// <summary>Connects the spawned cinematic camera to the current caster/ability.</summary>
        void Bind(AbilityContext context);

        /// <summary>Releases runtime camera bindings before the prefab is destroyed.</summary>
        void Unbind();
    }
}
