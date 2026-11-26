namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Optional adapter contract for camera-rig prefabs.
    /// Implement it in a component that references the Cinemachine version used by the game.
    /// </summary>
    public interface IAbilityCinematicCameraBinder
    {
        /// <summary>Connects the spawned camera rig to the current caster/ability.</summary>
        void Bind(AbilityContext context);

        /// <summary>Releases runtime camera bindings before the prefab is destroyed.</summary>
        void Unbind();
    }
}