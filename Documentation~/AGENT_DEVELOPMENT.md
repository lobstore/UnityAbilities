# Agent Guide: Designing and Implementing New Abilities

This guide is for coding agents that must **create or change gameplay abilities and their implementation**, not merely activate abilities that already exist. It is the implementation contract for this Unity package.

For agents that only need to choose, start, cancel, or interrupt existing abilities, use [Agent API Reference](AGENT_API.md) instead.

## 1. Understand the architecture before editing

The package separates an ability's authored data from its per-activation behavior:

- `AbilityData` is the top-level ScriptableObject. It stores the ability name, cooldown, and one configuration reference for each built-in phase.
- `AbilityStateStrategyConfig` and its phase-specific subclasses are ScriptableObject configuration assets. They hold designer-editable parameters and create runtime strategy objects.
- `IAbilityStateStrategy` is the runtime contract for a phase: `OnEnter`, `OnUpdate`, `OnExit`, `CanCancel`, and `CanBeInterrupted`.
- `AbilityState` wraps one runtime strategy. `AbilityStateMachine` enters, updates, transitions, and finishes states.
- `AbilityContext` carries the caster, current target and point, Animator, AbilityRuntime, cast origin, and owning state machine.
- `AbilityController` is the sample MonoBehaviour that starts configured abilities and forwards animation events. It is a sample integration, not a mandatory dependency for every game.

The standard flow is:

```text
Warmup -> Targeting -> Casting -> Release -> Finished
```

A strategy may transition earlier or skip a phase when the ability design requires it.

## 2. Decide whether a new strategy or a new core state is needed

**Default rule: implement a new strategy/config pair; do not add a new core state for every new ability.**

The built-in phases are designed to host different ability behaviors:

| Desired behavior | Preferred extension point |
| --- | --- |
| Charge-up animation, sound, startup VFX | Warmup config/runtime |
| Ground cursor, self-target, nearest target, cone selection | Targeting config/runtime |
| Channeling, cast-time progress, animation-event timing | Casting config/runtime |
| Projectile, melee hit, healing, AoE, summon, buff application | Release config/runtime |

Create a new core `AbilityState` type only when the project genuinely needs a new lifecycle phase with distinct transition semantics shared by multiple abilities. That is a core framework change: update `AbilityData`, state construction/transition code, controller integration, samples, both language guides, and tests. Do not use a new state type to represent a one-off ability effect.

## 3. Implement a new phase strategy

Each `AbilityData` field has a phase-specific type. A custom config must inherit from the matching class so it can be assigned in the Inspector:

- Warmup: `WarmupStrategyConfig`
- Targeting: `TargetingStrategyConfig`
- Casting: `CastingStrategyConfig`
- Release: `ReleaseStrategyConfig`

A custom config normally adds serialized parameters and overrides `CreateRuntime()`. Keep runtime-only data (elapsed time, selected target caches, spawned object handles, animation-event flags) in the runtime strategy, not in the ScriptableObject. `CreateRuntime()` must return a fresh instance for every ability activation.

### Example: custom Targeting phase

Config:

```csharp
using UnityEngine;

namespace LOGIYGames.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/Strategies/Targeting/Nearest Target")]
    public sealed class NearestTargetingConfig : TargetingStrategyConfig
    {
        [SerializeField, Min(0f)] private float _radius = 8f;
        [SerializeField] private LayerMask _targetLayers;

        public float Radius => _radius;
        public LayerMask TargetLayers => _targetLayers;

        public override IAbilityStateStrategy CreateRuntime()
            => new NearestTargetingRuntime(this);
    }
}
```

Runtime:

```csharp
using UnityEngine;

namespace LOGIYGames.Abilities
{
    public sealed class NearestTargetingRuntime : TargetingStrategyRuntime
    {
        private readonly NearestTargetingConfig _config;

        public NearestTargetingRuntime(NearestTargetingConfig config) : base(config)
            => _config = config;

        protected override void UpdateTargeting(AbilityContext ctx)
        {
            Collider[] hits = Physics.OverlapSphere(
                ctx.Caster.transform.position,
                _config.Radius,
                _config.TargetLayers);

            Transform nearest = null;
            float nearestDistance = float.PositiveInfinity;

            foreach (Collider hit in hits)
            {
                if (hit == null || hit.gameObject == ctx.Caster)
                    continue;

                float distance = (hit.transform.position - ctx.Caster.transform.position).sqrMagnitude;
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = hit.transform;
                }
            }

            ctx.Target = nearest != null ? nearest.gameObject : null;
            ctx.TargetPoint = nearest != null
                ? nearest.position
                : ctx.Caster.transform.position + ctx.Caster.transform.forward * _config.Radius;
        }
    }
}
```

This example inherits the built-in targeting lifecycle and only replaces target selection. It automatically keeps the base timing, cancel/interruption policy, VFX/SFX, animation, and transition to Casting. If the ability needs different lifecycle behavior, override the relevant virtual method or implement `IAbilityStateStrategy` directly.

### Implementing Warmup or Casting from scratch

The existing `WarmupStrategyRuntime` and `CastingStrategyRuntime` implement `IAbilityStateStrategy`, but their lifecycle methods are not virtual. For custom behavior in those phases, create a config deriving from `WarmupStrategyConfig` or `CastingStrategyConfig`, then return a custom runtime that implements `IAbilityStateStrategy` (and `IAnimationEventSink` when animation events are needed). Do not hide non-virtual methods with `new` and expect the state machine to call them polymorphically.

### Implementing Release

For a release phase that is a variation of the existing release behavior, derive the runtime from `ReleaseStrategyRuntime` and call `base.OnEnter(ctx)` to preserve shared VFX/SFX/animation behavior. If you override `OnExit(ctx)`, call `base.OnExit(ctx)` unless you intentionally replace the package's cooldown behavior. The base Release `OnExit` starts the ability cooldown.

## 4. Runtime lifecycle contract

Implement every strategy method deliberately:

| Method | Responsibility |
| --- | --- |
| `OnEnter(ctx)` | Reset per-activation fields, cache references, start animation/VFX/audio, subscribe to owned events if needed. |
| `OnUpdate(ctx)` | Advance timers and ability logic; request a state transition when this phase is complete. |
| `OnExit(ctx)` | Stop animation flags, unsubscribe, release temporary resources, and clean up objects owned by this phase. |
| `CanCancel(ctx)` | Return whether a cancellation request is currently allowed. Keep it side-effect free. |
| `CanBeInterrupted(ctx)` | Return whether another ability may replace this phase. Keep it side-effect free. |

Use `ctx.OwnerMachine.ChangeState(new CastingState(...))` (or the appropriate next state) to transition. Use `ctx.OwnerMachine.Finish()` only when the ability has completed normally or when the implementation intentionally requires unconditional termination. Gameplay cancellation and ability replacement should go through the policy-checked public APIs.

### Per-state cancellation and interruption

Every built-in config inherits these independent settings from `AbilityStateStrategyConfig`:

- `AllowCancel`: accept an explicit cancel request during this phase.
- `MinTimeBeforeCancel`: minimum time elapsed since this phase was entered before cancellation is accepted.
- `IsInterruptible`: allow a different ability to replace this ability during this phase.

The built-in runtime strategies use these settings. A custom implementation must preserve the same contract: `CanCancel` should honor `AllowCancel` and `MinTimeBeforeCancel` unless the ability design explicitly calls for a different rule; `CanBeInterrupted` should honor `IsInterruptible` unless intentionally overridden. Do not conflate cancellation with interruption.

A successful cancel or interrupt calls the active strategy's `OnExit`. This stops the state machine but does not automatically undo spawned projectiles, damage, network requests, or other external effects. Decide which objects are owned by the ability and clean them up in `OnExit` when cancellation should remove them. Effects that are intended to persist after release should be detached/transferred to a separate owner.

## 5. Use AbilityContext correctly

The available runtime context is:

| Member | Intended use |
| --- | --- |
| `Caster` | Character/GameObject executing the ability. |
| `Target` | Selected target GameObject; may be null. |
| `TargetPoint` | World-space destination/impact point. |
| `Animator` | Caster Animator; may be null. |
| `Ability` | Current `AbilityRuntime`; read ability data through `ctx.Ability.Data`. |
| `CastOrigin` | Transform used as the projectile/melee origin; validate before dereferencing. |
| `OwnerMachine` | State machine for transitions or normal finish. |

Do not assume a target exists. Define fallback behavior for self-targeted, ground-targeted, and no-target cases. Validate optional Unity references before using them. Avoid using `Camera.main` without checking that a main camera exists; prefer injecting a camera/reference if the project needs deterministic or multiplayer-safe targeting.

## 6. Animation events

A runtime strategy that needs animation events should implement `IAnimationEventSink`:

```csharp
public sealed class CustomCastingRuntime : IAbilityStateStrategy, IAnimationEventSink
{
    private bool _releaseEventReceived;

    public void OnEnter(AbilityContext ctx) => _releaseEventReceived = false;

    public void OnAnimationEvent(string eventName)
    {
        if (eventName == "Release")
            _releaseEventReceived = true;
    }

    // Implement OnUpdate, OnExit, CanCancel, and CanBeInterrupted.
}
```

The sample `AbilityController` forwards animation event names to the active strategy when it implements this interface. Ensure the event name in the Animator clip exactly matches the configured name. Reset event flags in `OnEnter`; stale events must not leak between activations.

The built-in configs also expose `ExitStateOnAnimationEvent` and `AnimationEventName`. Use these for the standard duration-or-event transition behavior where appropriate.

## 7. Assemble the AbilityData asset

After adding scripts:

1. Create the custom config assets through their `CreateAssetMenu` entries.
2. Create an `AbilityData` asset using **Assets → Create → Abilities → Ability Data**.
3. Set `AbilityName` and `CooldownDuration`.
4. Assign all four required phase config references: Warmup, Targeting, Casting, and Release. Reuse built-in configs for phases that need no custom behavior.
5. Configure per-state cancellation and interruption independently.
6. Assign the `AbilityData` to the game's ability loadout/controller.

The current `AbilityData` model expects all four phase references. Do not leave a required reference null unless you also add explicit null-safe transition behavior to the framework.

## 8. Repository conventions and file placement

Follow the existing sample layout:

- Runtime strategy code: `Samples/Example/Scripts/Strategies/<Phase>/<AbilityName>Runtime.cs`
- Config ScriptableObject: `Samples/Example/Scripts/Strategies/<Phase>/Factories/<AbilityName>Config.cs`
- Reusable package behavior: `Runtime/Core/Strategies/` and `Runtime/Factories/` only when the behavior belongs in the general framework rather than one sample/game ability.
- Documentation: update `Documentation~/en/README.md` and `Documentation~/ru/README.md` when a public workflow or setting changes.

Use the namespace `LOGIYGames.Abilities` for package/sample implementations consistent with the current code. Add Unity `.meta` files for new Unity assets/scripts when working in a local Unity checkout; let Unity generate valid GUIDs rather than copying another file's GUID.

## 9. Validation checklist

Before reporting an ability as implemented:

- [ ] The ability's expected gameplay behavior and phase responsibilities are written down.
- [ ] Each custom config inherits from the correct phase config type.
- [ ] Every `CreateRuntime()` returns a new runtime instance.
- [ ] Runtime fields and animation-event flags reset on every `OnEnter`.
- [ ] All required phase configs are assigned in the `AbilityData` asset.
- [ ] Cancellation and interruption are configured independently for every phase.
- [ ] Every `IAbilityStateStrategy` method is implemented and has deliberate behavior.
- [ ] `OnExit` restores Animator flags and cleans up owned resources.
- [ ] Null/no-target behavior and fallback target points are defined.
- [ ] Normal transitions, cancellation, and interruption are exercised.
- [ ] Cooldown behavior is checked. By default, cooldown is applied when Release exits; cancelling before Release does not automatically start cooldown.
- [ ] The package compiles in the target Unity version.
- [ ] Play Mode validation covers the ability's normal flow and its cancellation/interruption cases.

This repository currently has a sample project but no dedicated automated test suite. Do not claim automated or Unity validation unless it was actually run.

## 10. Expected agent delivery

When asked to implement an ability, provide:

1. A short behavior/phase breakdown.
2. The config and runtime scripts required for the ability.
3. Any new or modified `AbilityData`/sample asset and relevant `.meta` files, when the working environment supports Unity assets.
4. Updated English/Russian documentation when public setup or behavior changes.
5. Validation results, clearly separating completed checks from checks that could not be run.

Do not stop after writing a controller call to activate an existing `AbilityRuntime`: that is integration/usage, not ability implementation.
