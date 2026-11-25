namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Optional interface for runtime strategies that need to receive Unity Animator event callbacks.
    /// The sample AbilityController forwards event names to the currently active strategy when it implements this interface.
    /// </summary>
    public interface IAnimationEventSink
    {
        /// <summary>
        /// Receives an animation event emitted by an Animator clip.
        /// </summary>
        /// <param name="eventName">Exact event name configured on the animation clip.</param>
        void OnAnimationEvent(string eventName);
    }
}
