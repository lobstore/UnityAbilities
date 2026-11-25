# Agent API Reference — Runtime Integration

> **Scope:** This document is for agents that need to inspect and control abilities that have already been implemented. It is not the guide for creating new abilities. For ability authoring, read [Ability Development Guide (EN)](AGENT_DEVELOPMENT.md) or [Руководство по разработке способностей (RU)](AGENT_DEVELOPMENT_RU.md).

This is a compact contract for AI gameplay agents, planners, and runtime integration code using LOGIYGames Abilities.

## Operating model

An ability is active while `AbilityController.IsCasting` is true. Its active phase is available through `AbilityController.CurrentState`. The current phase decides whether it can be cancelled or interrupted.

Do not infer permissions from a state type name. Read the live capability properties or call the policy-checked methods.

## API contract

| Intent | API | Expected result |
| --- | --- | --- |
| Start an idle ability | `controller.TryActivateAbility(ability)` | `true` only if the ability starts. |
| Cancel the active ability | `controller.TryCancelCurrentAbility()` | `true` only if the current state's `CanCancel` policy accepted the request. |
| Interrupt without replacement | `controller.TryInterrupt()` | `true` only if the current state's `CanBeInterrupted` policy accepted the request. |
| Replace the active ability | `controller.TryActivateAbility(nextAbility, interruptCurrent: true)` | `true` only if the old ability was interruptible and the new one was ready and started. |
| Check cancellation capability | `controller.CanCancelCurrentAbility` | Live boolean capability. |
| Check interruption capability | `controller.CanInterruptCurrentAbility` | Live boolean capability. |
| Inspect phase | `controller.CurrentState` | Current `AbilityState`, or null. |

The equivalent low-level API is available on `AbilityStateMachine`: `CanCancel`, `CanBeInterrupted`, `TryCancel()`, and `TryInterrupt()`.

## Agent decision procedure

1. Check `controller.IsCasting`.
2. If no ability is active, call `TryActivateAbility(candidate)`.
3. If an ability is active and should stop without replacement, check/call `TryCancelCurrentAbility()`.
4. If an ability should be replaced, call `TryActivateAbility(candidate, interruptCurrent: true)`. Do not call `TryInterrupt()` first: the replacement overload performs the check and transition at the controller API level.
5. Treat a `false` result as a denied or failed action. Do not assume the ability stopped or the next one started.

Example:

```csharp
public bool AgentRequestAbility(AbilityController controller, AbilityRuntime requested)
{
    if (controller == null || requested == null)
        return false;

    if (!controller.IsCasting)
        return controller.TryActivateAbility(requested);

    return controller.TryActivateAbility(requested, interruptCurrent: true);
}
```

## Policy configuration

All state config assets derive from `AbilityStateStrategyConfig`:

- `AllowCancel`: enables explicit cancellation from this state.
- `MinTimeBeforeCancel`: minimum seconds elapsed since entering this state before cancellation succeeds.
- `IsInterruptible`: allows a different ability to replace the current one during this state.

These flags are independent and default to `false`. Policy decisions can also be specialized by overriding `CanCancel(AbilityContext)` or `CanBeInterrupted(AbilityContext)` in a custom strategy/state.

## Runtime safety rules

- Prefer `TryCancelCurrentAbility` and `TryInterrupt`; do not call `AbilityStateMachine.Finish()` to bypass gameplay policy.
- A `false` return means the requested transition did not happen.
- Do not assume interruption cleans up projectiles, spawned objects, network state, or other external side effects. Those are the owning strategy's responsibility, usually in `OnExit`.
- The Release state's `OnExit` applies cooldown, including when the state exits due to cancellation/interruption.
- Read the actual capability at decision time. It can change as `MinTimeBeforeCancel` elapses.
