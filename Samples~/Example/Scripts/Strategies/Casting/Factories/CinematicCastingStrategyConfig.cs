using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Configuration for a cinematic Casting phase that owns a temporary Cinemachine prefab.
    /// </summary>
    [CreateAssetMenu(menuName = "Abilities/Strategies/Casting/Cinemachine")]
    public class CinematicCastingStrategyConfig : CastingStrategyConfig
    {
        /// <summary>Prefab containing the Cinemachine camera setup to spawn for this Casting phase.</summary>
        public GameObject CinemachinePrefab;

        /// <summary>Whether to parent the spawned camera prefab to the caster transform.</summary>
        public bool ParentToCaster = true;

        /// <summary>Creates a cinematic Casting runtime for one ability activation.</summary>
        public override IAbilityStateStrategy CreateRuntime()
            => new CinematicCastingStrategyRuntime(this);
    }
}
