using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Owns a temporary camera-rig prefab for one active ability phase.
    /// The runtime has no direct dependency on a particular Cinemachine version.
    /// </summary>
    internal sealed class AbilityCinematicCameraRuntime
    {
        private readonly List<IAbilityCinematicCameraBinder> _cameraBinders = new();
        private GameObject _cameraInstance;

        /// <summary>Instantiates the configured camera rig and binds any adapter components.</summary>
        public void OnEnter(AbilityContext context, AbilityStateStrategyConfig config)
        {
            CleanupCamera();

            if (config == null || config.CinemachinePrefab == null)
                return;

            Transform caster = context != null && context.Caster != null
                ? context.Caster.transform
                : null;

            _cameraInstance = config.ParentToCaster && caster != null
                ? UnityEngine.Object.Instantiate(config.CinemachinePrefab, caster, false)
                : UnityEngine.Object.Instantiate(config.CinemachinePrefab);

            try
            {
                MonoBehaviour[] components = _cameraInstance.GetComponentsInChildren<MonoBehaviour>(true);
                foreach (MonoBehaviour component in components)
                {
                    if (!(component is IAbilityCinematicCameraBinder binder))
                        continue;

                    _cameraBinders.Add(binder);
                    binder.Bind(context);
                }
            }
            catch
            {
                CleanupCamera();
                throw;
            }
        }

        /// <summary>Unbinds adapters and destroys the temporary camera rig.</summary>
        public void OnExit()
        {
            CleanupCamera();
        }

        private void CleanupCamera()
        {
            try
            {
                for (int i = _cameraBinders.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        _cameraBinders[i].Unbind();
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                    }
                }
            }
            finally
            {
                _cameraBinders.Clear();

                if (_cameraInstance != null)
                {
                    UnityEngine.Object.Destroy(_cameraInstance);
                    _cameraInstance = null;
                }
            }
        }
    }
}