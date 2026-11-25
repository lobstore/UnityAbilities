namespace LOGIYGames.Abilities
{
    /// <summary>
    /// Controls the lifecycle of one ability activation, including state transitions,
    /// policy-checked cancellation, interruption, and completion.
    /// </summary>
    public class AbilityStateMachine
    {
        private AbilityState _current;
        private readonly AbilityContext _ctx;

        /// <summary>
        /// Gets whether this ability activation has finished.
        /// </summary>
        public bool IsFinished { get; private set; }

        /// <summary>
        /// Gets the active state, or <see langword="null"/> when no state is active.
        /// </summary>
        public AbilityState Current => _current;

        /// <summary>
        /// Gets whether the active state currently allows cancellation.
        /// The result can change while a state is active, for example when its
        /// minimum cancellation delay has elapsed.
        /// </summary>
        public bool CanCancel => !IsFinished && _current != null && _current.CanCancel(_ctx);

        /// <summary>
        /// Gets whether the active state currently allows another ability to interrupt it.
        /// </summary>
        public bool CanBeInterrupted
            => !IsFinished && _current != null && _current.CanBeInterrupted(_ctx);

        /// <summary>
        /// Creates a state machine for a single ability activation.
        /// </summary>
        /// <param name="ctx">Context shared by all states in this ability activation.</param>
        public AbilityStateMachine(AbilityContext ctx) => _ctx = ctx;

        /// <summary>
        /// Exits the current state and enters the supplied state.
        /// Calls are ignored after the machine has finished.
        /// </summary>
        /// <param name="next">State to enter. Passing <see langword="null"/> leaves the machine without an active state.</param>
        public void ChangeState(AbilityState next)
        {
            if (IsFinished)
                return;

            _current?.Exit(_ctx);
            _current = next;
            _current?.Enter(_ctx);
        }

        /// <summary>
        /// Updates the active state once.
        /// </summary>
        /// <param name="deltaTime">Frame time in seconds. Current built-in strategies read <see cref="UnityEngine.Time.deltaTime"/> directly.</param>
        public void Update(float deltaTime)
        {
            if (IsFinished || _current == null)
                return;

            _current.Update(_ctx);
        }

        /// <summary>
        /// Requests cancellation of the current ability, subject to the active state's policy.
        /// </summary>
        /// <returns><see langword="true"/> if the ability was cancelled; otherwise, <see langword="false"/> and the state remains active.</returns>
        public bool TryCancel()
        {
            if (!CanCancel)
                return false;

            Finish();
            return true;
        }

        /// <summary>
        /// Requests interruption of the current ability so another ability can take over,
        /// subject to the active state's interruption policy.
        /// </summary>
        /// <returns><see langword="true"/> if the ability was interrupted; otherwise, <see langword="false"/> and the state remains active.</returns>
        public bool TryInterrupt()
        {
            if (!CanBeInterrupted)
                return false;

            Finish();
            return true;
        }

        /// <summary>
        /// Immediately finishes this ability regardless of cancellation or interruption policies.
        /// The active state's <see cref="AbilityState.Exit"/> callback is invoked at most once.
        /// Prefer <see cref="TryCancel"/> or <see cref="TryInterrupt"/> for gameplay requests.
        /// </summary>
        public void Finish()
        {
            if (IsFinished)
                return;

            IsFinished = true;
            var previous = _current;
            try
            {
                // Keep Current available while the exiting strategy performs cleanup.
                previous?.Exit(_ctx);
            }
            finally
            {
                _current = null;
            }
        }
    }
}
