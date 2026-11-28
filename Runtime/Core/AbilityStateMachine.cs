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
            if (_current == next) return;
            _current?.Exit();
            _current = next;
            StateChanged?.Invoke();
            _current?.Enter();
        }

        public void Update()
        {
            if (_current != null) _current.Update();
        }

        public bool TryCancel()
        {
            if (_current == null || !_current.CanBeCancelled()) return false;
            Finish(AbilityEndReason.Cancelled);
            return true;
        }

        public bool TryInterrupt()
        {
            if (_current == null || !_current.CanBeInterrupted()) return false;
            Finish(AbilityEndReason.Interrupted);
            return true;
        }

        public void Finish() => Finish(AbilityEndReason.Completed);

        public void Finish(AbilityEndReason reason)
        {
            if (_current == null) return;
            AbilityContext context = _current.Context;
            _current.Exit();
            _current = null;
            context?.Services?.EndAbility(reason);
            Finished?.Invoke();
        }
    }
}