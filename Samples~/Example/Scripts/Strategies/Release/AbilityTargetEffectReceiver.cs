using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Example target component that exposes an event when an ability is applied.
    /// Replace or extend this component with game-specific health, status-effect,
    /// resistance, or interaction logic.
    /// </summary>
    public class AbilityTargetEffectReceiver : MonoBehaviour, IAbilityTargetEffectReceiver
    {
        /// <summary>Raised whenever a selected-target release is applied to this object.</summary>
        /// <summary>Unity event invoked after an ability has been applied to this target.</summary>
        public UnityEvent OnAbilityApplied = new();

        /// <summary>Invokes the example event. The supplied context can be used by custom receivers.</summary>
        /// <summary>Raises the Unity event so designers can connect target-side effects in the Inspector.</summary>
        /// <param name="context">Context of the ability being applied.</param>
        public void ApplyAbility(AbilityContext context)
        {
            OnAbilityApplied.Invoke();
        }
    }
}
