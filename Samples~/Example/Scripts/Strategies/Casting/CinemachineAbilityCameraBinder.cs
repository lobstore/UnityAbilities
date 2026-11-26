using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Optional version-independent binder for Cinemachine 2.x and 3.x camera components.
    /// Place this component on the configured camera prefab. It assigns the caster to
    /// Cinemachine components that expose Transform Follow and LookAt properties.
    /// </summary>
    public class CinemachineAbilityCameraBinder : MonoBehaviour, IAbilityCinematicCameraBinder
    {
        private readonly List<Component> _boundCameras = new();

        /// <summary>
        /// Assigns the caster transform to Follow and LookAt on all Cinemachine camera
        /// components found under this prefab.
        /// </summary>
        /// <param name="context">Ability context containing the caster transform.</param>
        public void Bind(AbilityContext context)
        {
            _boundCameras.Clear();

            if (context == null || context.Caster == null)
                return;

            foreach (Component component in GetComponentsInChildren<Component>(true))
            {
                if (component == null || !IsCinemachineComponent(component))
                    continue;

                bool bound = SetTransformProperty(component, "Follow", context.Caster.transform);
                bound |= SetTransformProperty(component, "LookAt", context.Caster.transform);

                if (bound)
                    _boundCameras.Add(component);
            }
        }

        /// <summary>
        /// Clears runtime targets from the bound cameras before the prefab is destroyed.
        /// </summary>
        public void Unbind()
        {
            foreach (Component component in _boundCameras)
            {
                if (component == null)
                    continue;

                SetTransformProperty(component, "Follow", null);
                SetTransformProperty(component, "LookAt", null);
            }

            _boundCameras.Clear();
        }

        private static bool IsCinemachineComponent(Component component)
        {
            string componentNamespace = component.GetType().Namespace;
            return !string.IsNullOrEmpty(componentNamespace) &&
                   componentNamespace.IndexOf("Cinemachine", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool SetTransformProperty(Component component, string propertyName, Transform value)
        {
            PropertyInfo property = component.GetType().GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public);

            if (property == null || !property.CanWrite || property.PropertyType != typeof(Transform))
                return false;

            try
            {
                property.SetValue(component, value);
                return true;
            }
            catch (TargetInvocationException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
