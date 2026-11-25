using System;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public class AnimationEventReceiver : MonoBehaviour
    {
        public event Action<string> OnAnimationEvent;

        // Вызывается из AnimationEvent в клипе
        public void Trigger(string eventName)
        {
            OnAnimationEvent?.Invoke(eventName);
        }
    }
}
