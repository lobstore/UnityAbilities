using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Input data consumed by targeting strategies. A player input component, AI,
    /// network authority, or scripted sequence can populate this object without
    /// making the targeting runtime depend on a Camera or Input API.
    /// </summary>
    public sealed class AbilityTargetingInput
    {
        /// <summary>Gets whether a world-space target position has been supplied.</summary>
        public bool HasWorldPosition { get; private set; }

        /// <summary>Gets the latest world-space position supplied by the input source.</summary>
        public Vector3 WorldPosition { get; private set; }

        /// <summary>Gets the currently selected target GameObject, or null when no object is selected.</summary>
        public GameObject SelectedTarget { get; private set; }

        /// <summary>
        /// Sets the world-space position used by position targeting.
        /// An optional target can be supplied when the position came from a hit test.
        /// </summary>
        /// <param name="worldPosition">World-space point to use for position-based targeting.</param>
        /// <param name="selectedTarget">Optional object hit/selected at that point.</param>
        public void SetPosition(Vector3 worldPosition, GameObject selectedTarget = null)
        {
            HasWorldPosition = true;
            WorldPosition = worldPosition;
            SelectedTarget = selectedTarget;
        }

        /// <summary>
        /// Sets the object selected by the player or AI and uses its current position
        /// as the targeting point. Passing null clears the selected object and position.
        /// </summary>
        /// <param name="selectedTarget">Object selected by the player or AI; null clears the selection.</param>
        public void SetSelectedTarget(GameObject selectedTarget)
        {
            SelectedTarget = selectedTarget;
            HasWorldPosition = selectedTarget != null;

            if (selectedTarget != null)
                WorldPosition = selectedTarget.transform.position;
            else
                WorldPosition = Vector3.zero;
        }

        /// <summary>
        /// Clears the currently supplied position and selected target.
        /// </summary>
        public void Clear()
        {
            HasWorldPosition = false;
            WorldPosition = Vector3.zero;
            SelectedTarget = null;
        }
    }
}
