# Система способностей

## Обзор

Способность выполняется через `AbilityStateMachine` и четыре настраиваемых состояния:

1. **Warmup** — подготовка, стартовая анимация и эффекты.
2. **Targeting** — выбор цели и расчёт точки применения.
3. **Casting** — время произнесения/подготовки способности.
4. **Release** — фактическое выполнение способности и запуск перезарядки.

Для каждого состояния создаётся runtime-стратегия на основе ScriptableObject-конфигурации. Отмена и прерывание настраиваются независимо для каждого состояния.

## Камеры и синематики

В каждой конфигурации фазы доступны общие поля `CinemachinePrefab` и `ParentToCaster`. Назначьте в `CinemachinePrefab` prefab с любым camera rig: Cinemachine Virtual Camera, Cinemachine Camera, Dolly или собственной композицией. Rig создаётся при входе в фазу и уничтожается при выходе из неё. `ParentToCaster` помещает prefab под исполнителя, сохраняя заданные в prefab локальные позицию и поворот. Например, для отдельного ракурса удобного наведения назначьте camera prefab в конфигурации Targeting. Основной runtime не зависит от конкретной версии Cinemachine; для настройки Follow/LookAt или цели во время выполнения добавьте компонент, реализующий `IAbilityCinematicCameraBinder`.

## Отмена и прерывание — в чём разница

| Операция | Назначение | Настройки |
| --- | --- | --- |
| Отмена (Cancel) | Явно прекратить текущую способность: например, по Escape или решению AI. | `AllowCancel`, `MinTimeBeforeCancel` |
| Прерывание (Interrupt) | Прекратить текущую способность, чтобы вместо неё запустить другую. | `IsInterruptible` |

Состояние может разрешать отмену, но запрещать прерывание, или наоборот. Это независимые флаги. По умолчанию все политики выключены — включайте их отдельно в конфигурации каждого состояния.

`MinTimeBeforeCancel` отсчитывается с момента входа в текущее состояние, а не с начала всей способности.

## Быстрый старт

Создайте asset `AbilityData`, назначьте необходимые конфигурации стратегий и добавьте `AbilityController` на GameObject персонажа. Назначьте ссылки на конфигурации способностей в инспекторе.

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

Чтобы запустить другую способность и прервать текущую, если состояние это разрешает:

```csharp
AbilityRuntime next = _controller.Abilities[1];
bool started = _controller.TryActivateAbility(next, interruptCurrent: true);
```

Если текущее состояние запрещает прерывание, текущая способность продолжит выполняться, а новая не запустится.

## Публичный API

### `AbilityController`

| Член API | Описание |
| --- | --- |
| `TryActivateAbility(AbilityRuntime ability, bool interruptCurrent = false)` | Запускает способность, если она готова. Если другая способность уже выполняется, заменить её можно только при `interruptCurrent: true` и разрешённом прерывании текущего состояния. |
| `TryCancelCurrentAbility()` | Запрашивает отмену с учётом политики текущего состояния. |
| `TryInterrupt()` | Запрашивает прерывание с учётом политики текущего состояния. Не запускает заменяющую способность. |
| `IsCasting` | True, пока выполняется машина состояний способности. |
| `CurrentState` | Объект текущего состояния или null, если активного состояния нет. |
| `CurrentAbility` | Runtime текущей выбранной способности. |
| `CanCancelCurrentAbility` | Разрешена ли отмена прямо сейчас. |
| `CanInterruptCurrentAbility` | Разрешено ли прерывание прямо сейчас. |
| `Abilities` | Доступный только для чтения список runtime способностей. |

### `AbilityStateMachine`

| Член API | Описание |
| --- | --- |
| `TryCancel()` | Отмена с проверкой политики состояния. Возвращает true, если способность завершена. |
| `TryInterrupt()` | Прерывание с проверкой политики состояния. Возвращает true, если способность завершена. |
| `CanCancel` | Текущий результат проверки возможности отмены. |
| `CanBeInterrupted` | Текущий результат проверки возможности прерывания. |
| `Finish()` | Безусловно завершает способность. Используйте для внутренней логики машины состояний или осознанного принудительного завершения. |

### Расширение состояний и стратегий

- `AbilityState.CanCancel(AbilityContext)` и `AbilityState.CanBeInterrupted(AbilityContext)` предоставляют проверки для конкретного состояния.
- `IAbilityStateStrategy.CanCancel(AbilityContext)` реализует политику отмены.
- `IAbilityStateStrategy.CanBeInterrupted(AbilityContext)` реализует политику прерывания.
- `AbilityStateStrategyConfig.AllowCancel`, `MinTimeBeforeCancel` и `IsInterruptible` — общие настройки для Warmup, Targeting, Casting и Release.

При создании собственной реализации `IAbilityStateStrategy` реализуйте оба метода проверки политик вместе с callback-методами жизненного цикла.

## Важные особенности

- Успешная отмена или прерывание один раз вызывает `OnExit` текущей стратегии и помечает машину состояний завершённой.
- Отклонённый запрос не меняет текущее состояние.
- `Finish()` обходит проверки политик. Для ввода игрока и решений AI используйте `TryCancel()` или `TryInterrupt()`.
- Release запускает перезарядку в `OnExit`. Поэтому выход из Release по завершению, отмене или прерыванию применяет cooldown.
- Отмена/прерывание останавливает машину состояний, но не удаляет автоматически уже созданные внешние эффекты. При необходимости реализуйте очистку в `OnExit` пользовательской стратегии.
- При успешном вызове `TryActivateAbility(next, interruptCurrent: true)` текущая способность завершается до старта следующей.

## Настройка конфигураций

Для каждой конфигурации Warmup, Targeting, Casting и Release:

1. Включите **Allow Cancel**, если состояние можно отменять вручную или по решению AI.
2. Настройте **Min Time Before Cancel**, чтобы запретить слишком раннюю отмену в этом состоянии.
3. Включите **Is Interruptible**, если во время состояния разрешено запустить другую способность вместо текущей.
4. Настраивайте отмену и прерывание независимо друг от друга.

## Совместимость

Настройки отмены перенесены в базовый класс `AbilityStateStrategyConfig`, чтобы все состояния использовали одну политику. После обновления проверьте существующие Targeting assets в Inspector и убедитесь, что значения **Allow Cancel** и **Min Time Before Cancel** сохранились. Для новых конфигураций отмена по умолчанию выключена.

## Требования

- Unity 2022.3 или новее.

## Для coding agents

Для проектирования и реализации новых способностей используйте [руководство по разработке способностей](../AGENT_DEVELOPMENT_RU.md). [Runtime Agent API](../AGENT_API.md) предназначен для управления уже существующими способностями.
