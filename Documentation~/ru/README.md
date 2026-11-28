# Система способностей

## Обзор

Способность выполняется через `AbilityStateMachine` и четыре настраиваемых состояния:

1. **Warmup** — подготовка, стартовая анимация и эффекты.
2. **Targeting** — выбор цели и расчёт точки применения.
3. **Casting** — время произнесения/подготовки способности.
4. **Release** — фактическое выполнение способности и запуск перезарядки.

Для каждого состояния создаётся runtime-стратегия на основе ScriptableObject-конфигурации. Отмена и прерывание настраиваются независимо для каждого состояния.

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

## Расширения

Новые runtime-модули, VFX/SFX, эффекты персонажей, повторения цикла и прогрессия уровней описаны в [руководстве Ability Modules](../ABILITY_MODULES_RU.md).

## Требования

- Unity 2022.3 или новее.

## Для coding agents

Для проектирования и реализации новых способностей используйте [руководство по разработке способностей](../AGENT_DEVELOPMENT_RU.md). [Runtime Agent API](../AGENT_API.md) предназначен для управления уже существующими способностями.
