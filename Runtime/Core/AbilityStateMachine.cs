namespace LOGIYGames.Abilities
{
    public class AbilityStateMachine
    {
        private AbilityState _current;
        private readonly AbilityContext _ctx;

        public bool IsFinished { get; private set; }
        public AbilityState Current => _current;

        /// <summary>
        /// True when the active state currently allows cancellation.
        /// </summary>
        public bool CanCancel => !IsFinished && _current != null && _current.CanCancel(_ctx);

        /// <summary>
        /// True when the active state currently allows interruption by another ability.
        /// </summary>
        public bool CanBeInterrupted
            => !IsFinished && _current != null && _current.CanBeInterrupted(_ctx);

        public AbilityStateMachine(AbilityContext ctx) => _ctx = ctx;

        public void ChangeState(AbilityState next)
        {
            if (IsFinished)
                return;

            _current?.Exit(_ctx);
            _current = next;
            _current?.Enter(_ctx);
        }

        public void Update(float deltaTime)
        {
            if (IsFinished || _current == null)
                return;

            _current.Update(_ctx);
        }

        /// <summary>
        /// Requests a regular cancellation. The active state's CanCancel policy is respected.
        /// </summary>
        /// <returns>True when the ability was cancelled.</returns>
        public bool TryCancel()
        {
            if (!CanCancel)
                return false;

            Finish();
            return true;
        }

        /// <summary>
        /// Requests interruption by another ability. The active state's
        /// CanBeInterrupted policy is respected.
        /// </summary>
        /// <returns>True when the ability was interrupted.</returns>
        public bool TryInterrupt()
        {
            if (!CanBeInterrupted)
                return false;

            Finish();
            return true;
        }

        /// <summary>
        /// Immediately finishes the ability regardless of the active state's policies.
        /// Use TryCancel or TryInterrupt for policy-controlled termination.
        /// </summary>
        public void Finish()
        {
            if (IsFinished)
                return;

            // Mark the machine as finished before invoking user strategy code.
            // This keeps Finish idempotent if OnExit causes another lifecycle call.
            IsFinished = true;
            var previous = _current;
            _current = null;
            previous?.Exit(_ctx);
        }
    }
}
