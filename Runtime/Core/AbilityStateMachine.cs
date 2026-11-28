using System;

namespace LOGIYGames.Abilities
{
    public class AbilityStateMachine
    {
        private AbilityState _current;
        public AbilityState CurrentState => _current;
        public event Action StateChanged;
        public event Action Finished;
        public void ChangeState(AbilityState next)
        {
            _current?.Exit();
            _current = next;
            StateChanged?.Invoke();
            _current?.Enter();
        }

        public void Update(float deltaTime)
        {
            if (_current == null)
                return;

            _current.Update();
        }

        /// <summary>
        /// Requests a regular cancellation. The active state's CanBeCancelled policy is respected.
        /// </summary>
        /// <returns>True when the ability was cancelled.</returns>
        public bool TryCancel()
        {
            if (_current == null) return false;
            if (_current.CanBeCancelled())
            {
                Finish();
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Requests interruption by another ability. The active state's
        /// CanBeInterrupted policy is respected.
        /// </summary>
        /// <returns>True when the ability was interrupted.</returns>
        public bool TryInterrupt()
        {
            if (_current == null) return false;

            if (_current.CanBeInterrupted())
            {
                Finish();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Immediately finishes the ability regardless of the active state's policies.
        /// Use TryCancel or TryInterrupt for policy-controlled termination.
        /// </summary>
        public void Finish()
        {
            if (_current == null) return;
            _current?.Exit();
            _current = null;
            Finished?.Invoke();
        }
    }
}
