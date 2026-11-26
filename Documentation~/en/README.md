# Abilities System

## Overview

An ability is executed by an `AbilityStateMachine` through four configurable states:

1. **Warmup** — startup animation/effects.
2. **Targeting** — target selection and target-point calculation.
3. **Casting** — cast-time animation/effects.
4. **Release** — ability execution and cooldown start.

Each state owns a runtime strategy created from its ScriptableObject configuration. Configure interruption and cancellation separately for every state.

## Camera rigs and cinematics

Every phase config exposes the shared `CinemachinePrefab` and `ParentToCaster` fields. Assign any camera-rig prefab to `CinemachinePrefab`: a Cinemachine Virtual Camera, Cinemachine Camera, Dolly rig, or a custom composition. The rig is instantiated when the phase is entered and destroyed when it exits. `ParentToCaster` parents the prefab to the caster while preserving its authored local position and rotation. For example, assign a dedicated camera prefab to the Targeting config to provide a more convenient aiming view. The core runtime has no dependency on a specific Cinemachine version; implement `IAbilityCinematicCameraBinder` to configure Follow/LookAt or target bindings at runtime.

## Cancel vs. interrupt

| Operation | Purpose | Configuration |
| --- | --- | --- |
| Cancel | Explicitly stop the current ability (for example, Escape, stun input, or an AI decision). | `AllowCancel`, `MinTimeBeforeCancel` |
| Interrupt | Stop the current ability to let another ability take over. | `IsInterruptible` |

A state can allow cancellation and reject interruption, or the opposite. Both flags are independent. All policies default to `false`; opt in on each state's config asset.

`MinTimeBeforeCancel` is measured from the moment the current state was entered. It is not the total ability elapsed time.

## Quick start

Create an `AbilityData` asset, assign all required strategy configs, and add `AbilityController` to the caster GameObject. Assign the controller's ability references.

```csharp
using LOGIYGames.Abilities;
using UnityEngine;

public sealed class AbilityInput : MonoBehaviour
{
    [SerializeField] private AbilityController _controller;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && _controller.Abilities.Count > 0)
            _controller.TryActivateAbility(_controller.Abilities[0]);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool cancelled = _controller.TryCancelCurrentAbility();
            Debug.Log($"Ability cancelled: {cancelled}");
        }
    }
}
```

To activate a different ability and interrupt the current one when permitted:

```csharp
AbilityRuntime next = _controller.Abilities[1];
bool started = _controller.TryActivateAbility(next, interruptCurrent: true);
```

If the current state does not allow interruption, the current ability remains active and the new ability is not started.

## Public API

### `AbilityController`

| Member | Description |
| --- | --- |
| `TryActivateAbility(AbilityRuntime ability, bool interruptCurrent = false)` | Starts the ability if it is ready. If another ability is active, it can be replaced only when `interruptCurrent` is true and the active state allows interruption. |
| `TryCancelCurrentAbility()` | Requests cancellation using the active state's cancellation policy. |
| `TryInterrupt()` | Requests interruption using the active state's interruption policy. Does not start a replacement ability. |
| `IsCasting` | True while an ability state machine is active. |
| `CurrentState` | Current state object, or null when no state is active. |
| `CurrentAbility` | Runtime of the currently selected ability. |
| `CanCancelCurrentAbility` | Whether cancellation is currently allowed. |
| `CanInterruptCurrentAbility` | Whether interruption is currently allowed. |
| `Abilities` | Read-only list of configured ability runtimes. |

### `AbilityStateMachine`

| Member | Description |
| --- | --- |
| `TryCancel()` | Policy-checked cancellation. Returns whether it finished the ability. |
| `TryInterrupt()` | Policy-checked interruption. Returns whether it finished the ability. |
| `CanCancel` | Current state's live cancellation decision. |
| `CanBeInterrupted` | Current state's live interruption decision. |
| `Finish()` | Unconditionally finishes the ability. Intended for state-machine internals or deliberate forced termination. |

### State and strategy extension points

- `AbilityState.CanCancel(AbilityContext)` and `AbilityState.CanBeInterrupted(AbilityContext)` expose per-state decisions.
- `IAbilityStateStrategy.CanCancel(AbilityContext)` implements the cancellation policy.
- `IAbilityStateStrategy.CanBeInterrupted(AbilityContext)` implements the interruption policy.
- `AbilityStateStrategyConfig.AllowCancel`, `MinTimeBeforeCancel`, and `IsInterruptible` are common settings available to Warmup, Targeting, Casting, and Release configs.

When implementing a custom `IAbilityStateStrategy`, implement both policy methods in addition to the lifecycle callbacks.

## Behavior notes

- A successful cancel or interrupt calls the active strategy's `OnExit` exactly once and marks the machine finished.
- A denied request does not change the active state.
- `Finish()` bypasses policy checks. Prefer `TryCancel()` or `TryInterrupt()` for gameplay input and AI decisions.
- Release applies cooldown in `OnExit`, so leaving Release by completion, cancellation, or interruption applies its configured cooldown.
- Cancellation/interruption stops the ability state machine; it does not automatically roll back external effects already spawned by a custom strategy. Custom strategies should implement cleanup in `OnExit` when needed.
- A successful interruption through `TryActivateAbility(next, interruptCurrent: true)` finishes the current ability before starting the next one.

## Configuration checklist

For each Warmup, Targeting, Casting, and Release config:

1. Enable **Allow Cancel** when manual/AI cancellation should be accepted in that state.
2. Set **Min Time Before Cancel** to prevent cancellation too early in that state.
3. Enable **Is Interruptible** when another ability may take over during that state.
4. Keep these choices independent; enabling one does not enable the other.

## Compatibility

Cancellation settings have been promoted to `AbilityStateStrategyConfig`, so every state uses one shared policy. Review existing Targeting assets in the Inspector after upgrading and confirm their **Allow Cancel** and **Min Time Before Cancel** values. Newly created config assets have cancellation disabled by default.

## Requirements

- Unity 2022.3 or later.

## For coding agents

To design and implement new abilities, read the [Ability Development Guide](../AGENT_DEVELOPMENT.md). The [Runtime Agent API](../AGENT_API.md) is for controlling abilities that already exist.
