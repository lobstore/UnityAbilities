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
        public UnityEvent OnAbilityApplied = new();

        /// <summary>Invokes the example event. The supplied context can be used by custom receivers.</summary>
        public void ApplyAbility(AbilityContext context)
        {
            OnAbilityApplied.Invoke();
        }
    }
}
