using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public enum AbilityResourceScope { Ability, State, Detached }

    public sealed class AbilityResourceHandle : IDisposable
    {
        private Action _release;
        private Action<AbilityResourceHandle> _onDisposed;
        private bool _isDisposed;
        public bool IsDisposed => _isDisposed;
        public AbilityResourceScope Scope { get; private set; }
        public int OwnerId { get; private set; }

        internal AbilityResourceHandle(Action release, Action<AbilityResourceHandle> onDisposed,
            AbilityResourceScope scope, int ownerId)
        {
            _release = release; _onDisposed = onDisposed; Scope = scope; OwnerId = ownerId;
        }

        internal void SetScope(AbilityResourceScope scope, int ownerId)
        {
            Scope = scope; OwnerId = ownerId;
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            Action release = _release;
            _release = null;
            try { release?.Invoke(); }
            finally { _onDisposed?.Invoke(this); _onDisposed = null; }
        }
    }

    public sealed class AbilityResourceRegistry : IDisposable
    {
        private readonly List<AbilityResourceHandle> _handles = new();
        private bool _disposed;

        public AbilityResourceHandle Register(IDisposable resource,
            AbilityResourceScope scope = AbilityResourceScope.Ability, int ownerId = 0)
            => resource == null ? null : Register(resource.Dispose, scope, ownerId);

        public AbilityResourceHandle Register(GameObject gameObject,
            AbilityResourceScope scope = AbilityResourceScope.Ability, int ownerId = 0)
        {
            if (gameObject == null) return null;
            return Register(() => DestroyGameObject(gameObject), scope, ownerId);
        }

        public AbilityResourceHandle Register(Action release,
            AbilityResourceScope scope = AbilityResourceScope.Ability, int ownerId = 0)
        {
            if (_disposed) { release?.Invoke(); return null; }
            AbilityResourceHandle handle = null;
            handle = new AbilityResourceHandle(release,
                disposedHandle => _handles.Remove(disposedHandle), scope, ownerId);
            _handles.Add(handle);
            return handle;
        }

        public void Detach(AbilityResourceHandle handle)
        {
            if (handle == null || handle.IsDisposed) return;
            handle.SetScope(AbilityResourceScope.Detached, 0);
        }

        public void ReleaseScope(AbilityResourceScope scope, int ownerId = 0)
        {
            AbilityResourceHandle[] snapshot = _handles.ToArray();
            foreach (AbilityResourceHandle handle in snapshot)
                if (handle.Scope == scope && (scope != AbilityResourceScope.State || handle.OwnerId == ownerId))
                    handle.Dispose();
        }

        public void ReleaseAll(bool includeDetached = false)
        {
            AbilityResourceHandle[] snapshot = _handles.ToArray();
            foreach (AbilityResourceHandle handle in snapshot)
                if (includeDetached || handle.Scope != AbilityResourceScope.Detached)
                    handle.Dispose();
        }

        public void Dispose()
        {
            if (_disposed) return;
            ReleaseAll();
            _handles.Clear();
            _disposed = true;
        }

        private static void DestroyGameObject(GameObject gameObject)
        {
            if (gameObject == null) return;
            if (Application.isPlaying) UnityEngine.Object.Destroy(gameObject);
            else UnityEngine.Object.DestroyImmediate(gameObject);
        }
    }
}