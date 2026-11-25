# Coding agent notes

## Project
- Unity package: `com.logiygames.abilities`
- Runtime assembly: `LOGIYGames.Abilities`
- Minimum Unity version: 2022.3.

## Ability lifecycle
- The standard flow is Warmup -> Targeting -> Casting -> Release.
- State transitions are coordinated by `AbilityStateMachine`.
- Runtime state behavior belongs in `IAbilityStateStrategy` implementations.
- Configuration assets derive from `AbilityStateStrategyConfig`.

## Cancellation and interruption API
- Explicit cancellation: `AbilityController.TryCancelCurrentAbility()`.
- Interruption without replacement: `AbilityController.TryInterrupt()`.
- Replace the running ability: `AbilityController.TryActivateAbility(next, interruptCurrent: true)`.
- Do not bypass policies with `AbilityStateMachine.Finish()` for ordinary gameplay/AI decisions.
- `AllowCancel` and `MinTimeBeforeCancel` govern cancellation for every state.
- `IsInterruptible` governs replacement by another ability for every state.
- Both policies must be checked at request time; do not infer them from the state class.

## When adding a state or strategy
- Implement `OnEnter`, `OnUpdate`, `OnExit`, `CanCancel`, and `CanBeInterrupted` in every `IAbilityStateStrategy`.
- Reset elapsed-time and animation-event fields in `OnEnter`.
- Clean up state-owned resources in `OnExit`.
- Keep policy checks side-effect free.
- Add examples and update both language guides when public API changes.

## References
- [Agent API contract](Documentation~/AGENT_API.md)
- [English guide](Documentation~/en/README.md)
- [Russian guide](Documentation~/ru/README.md)
