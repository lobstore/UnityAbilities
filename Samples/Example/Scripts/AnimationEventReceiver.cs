using System;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// MonoBehaviour bridge that exposes Unity Animation Events as a C# event
    /// for the active ability strategy.
    /// </summary>
    public class AnimationEventReceiver : MonoBehaviour
    {
        /// <summary>
        /// Raised when an animation clip calls <see cref="Trigger"/>.
        /// The string argument is the event name configured in the clip.
        /// </summary>
        public event Action<string> OnAnimationEvent;

        /// <summary>
        /// Entry point for a Unity Animation Event. Forwards the event name
        /// to all subscribers, such as the ability controller.
        /// </summary>
        /// <param name="eventName">Exact event name configured on the animation clip.</param>
        public void Trigger(string eventName)
        {
            OnAnimationEvent?.Invoke(eventName);
        }
    }
}
