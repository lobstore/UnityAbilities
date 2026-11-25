using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Casting strategy that spawns a Cinemachine camera prefab on entry and
    /// destroys it on exit. The prefab can contain a version-specific binder
    /// implementing IAbilityCinematicCameraBinder.
    /// </summary>
    public class CinematicCastingStrategyRuntime : CastingStrategyRuntime
    {
        private readonly CinematicCastingStrategyConfig _cinematicConfig;
        private readonly List<IAbilityCinematicCameraBinder> _cameraBinders = new();
        private GameObject _cameraInstance;

        /// <summary>Creates a casting runtime that owns a temporary Cinemachine prefab instance.</summary>
        public CinematicCastingStrategyRuntime(CinematicCastingStrategyConfig config) : base(config)
        {
            _cinematicConfig = config;
        }

        /// <summary>Starts normal Casting behavior and instantiates/binds the cinematic camera prefab.</summary>
        public override void OnEnter(AbilityContext ctx)
        {
            base.OnEnter(ctx);

            if (_cinematicConfig.CinemachinePrefab == null)
                return;

            Transform caster = ctx.Caster != null ? ctx.Caster.transform : null;
            Transform parent = _cinematicConfig.ParentToCaster ? caster : null;
            Vector3 position = caster != null ? caster.position : Vector3.zero;
            Quaternion rotation = caster != null ? caster.rotation : Quaternion.identity;

            _cameraInstance = Object.Instantiate(
                _cinematicConfig.CinemachinePrefab,
                position,
                rotation,
                parent);

            _cameraBinders.Clear();
            try
            {
                foreach (MonoBehaviour component in _cameraInstance.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    if (component is IAbilityCinematicCameraBinder binder)
                    {
                        _cameraBinders.Add(binder);
                        binder.Bind(ctx);
                    }
                }
            }
            catch
            {
                CleanupCamera();
                throw;
            }
        }

        /// <summary>Unbinds and destroys the cinematic camera instance, then exits the base Casting phase.</summary>
        public override void OnExit(AbilityContext ctx)
        {
            try
            {
                CleanupCamera();
            }
            finally
            {
                base.OnExit(ctx);
            }
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
                    catch (System.Exception exception)
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
                    Object.Destroy(_cameraInstance);
                    _cameraInstance = null;
                }
            }
        }
    }
}
