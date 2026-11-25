# LOGIYGames Abilities

A configurable state-based abilities system for Unity 2022.3+.

## Documentation

- [English user guide](Documentation~/en/README.md)
- [Руководство пользователя на русском](Documentation~/ru/README.md)
- [Ability development guide for coding agents (EN)](Documentation~/AGENT_DEVELOPMENT.md)
- [Руководство для coding agents по разработке способностей (RU)](Documentation~/AGENT_DEVELOPMENT_RU.md)
- [Runtime Agent API reference](Documentation~/AGENT_API.md)
- [Repository instructions for coding agents](AGENTS.md)

## Quick start

Create an `AbilityData` asset and configure its Warmup, Targeting, Casting, and Release strategies. Add `AbilityController` to the caster and assign the ability assets.

```csharp
using LOGIYGames.Abilities;
using UnityEngine;

public sealed class AbilityInput : MonoBehaviour
{
    [SerializeField] private AbilityController _abilities;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && _abilities.Abilities.Count > 0)
            _abilities.TryActivateAbility(_abilities.Abilities[0]);

        if (Input.GetKeyDown(KeyCode.Escape))
            _abilities.TryCancelCurrentAbility();
    }
}
```

To replace an active ability, pass `interruptCurrent: true`. The current state must have `IsInterruptible` enabled.

```csharp
_abilities.TryActivateAbility(nextAbility, interruptCurrent: true);
```

Cancellation and interruption are independent per-state policies:
- `AllowCancel` and `MinTimeBeforeCancel` govern player/AI cancellation requests.
- `IsInterruptible` governs whether a different ability may interrupt the active one.

Both operations return `true` only when the active ability was actually terminated.
