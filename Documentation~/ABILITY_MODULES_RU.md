# Ability Modules, Effects, Flow и Progression

Это руководство описывает расширения runtime API. Factory хранит только сериализуемые настройки; изменяемое состояние конкретной активации хранится в runtime-модулях.

## Точки подключения

- `AbilityDefinition.Modules` — модули на всё время одной активации.
- `AbilityStateStrategyFactory.Modules` — модули одной фазы.
- `AbilityObjectStrategyFactory.Modules` — модули отдельного `AbilityObject`.
- `AbilityDefinition.FlowModules` — правила перехода между фазами.
- `AbilityDefinition.Progression` — настройки уровней.

Списки модулей используют `SerializeReference`. В Inspector добавляйте конкретные классы из списка managed references.

## События

`AbilityEventDispatcher` публикует `AbilityStarted`, `StateEntered`, `StateExited`, `AnimationEvent`, `IterationStarted`, `ReleasePerformed`, `AbilityCancelled`, `AbilityInterrupted`, `AbilityEnded`, `AbilityLevelChanged` и `ObjectCollision`.

Наследуйте `AbilityModuleRuntime` и переопределяйте `OnEvent`, `Tick` и `Dispose`. Runtime создаётся заново через factory при каждом запуске способности.

## Встроенные presentation-модули

### SpawnVFXModuleFactory

Настройте prefab, trigger, фильтр фазы, позицию, anchor, lifetime и `ResourceScope`.

### SpawnAttachedObjectModuleFactory

Создаёт объект при старте способности и прикрепляет к персонажу или именованной дочерней точке. Для фантомного меча задайте prefab, `AnchorName = RightHand` и `ResourceScope = Ability`; объект будет очищен при завершении способности.

### PlaySFXModuleFactory

Воспроизводит AudioClip при выбранном событии. С назначенным AudioSource используется `PlayOneShot`; без источника создаётся временный AudioSource рядом с кастером.

## Gameplay Effects

Добавьте `AbilityEffectController` на персонажа или цель. Компонент реализует `IAbilityEffectReceiver` и отслеживает duration/stacking для активных эффектов.

`ApplyEffectModuleFactory` выбирает кастера, первую цель или все цели. Модуль в `AbilityDefinition.Modules` может слушать события на протяжении способности; модуль в фазовой конфигурации ограничен временем жизни фазы. Нефильтрованный `StateEntered` подходит, когда действие должно срабатывать при входе в каждую фазу.

`AbilityEffectDefinition` — расширяемая ScriptableObject-конфигурация. Для собственного баффа/дебаффа унаследуйте её и реализуйте `CreateRuntime`; runtime эффекта реализует `OnApply`, `Tick`, `OnRemove`. Встроенный `TimedVisualEffectDefinition` демонстрирует визуальный эффект на выбранной цели.

Политики наложения: `Stack`, `Refresh`, `Ignore`. Duration равный нулю означает отсутствие автоматического тайм-аута; снятие в этом случае выполняется по освобождению handle или уничтожению EffectController.

## Создание объектов с эффектами

`SpawnAbilityObjectModuleFactory` создаёт prefab по событию. Если на объекте есть `AbilityObject`, модуль передаёт ему `AbilityObjectContext`. В поле `InitialEffects` можно назначить эффекты, которые будут применены к самому созданному объекту. Для внутренних реакций добавляйте модули в `AbilityObjectStrategyFactory.Modules`; события столкновения получают только модули соответствующего объекта.

## Владение ресурсами

`AbilityResourceRegistry` поддерживает три области:

- `Ability` — очищается при завершении способности.
- `State` — очищается при выходе из конкретной фазы.
- `Detached` — не очищается вместе с родительской способностью.

Регистрируйте GameObject, IDisposable или callback освобождения. Для передачи уже зарегистрированного ресурса за пределы scope используйте `Resources.Detach(handle)`. Снаряд, которому нужно пережить завершение каста, должен иметь собственный lifetime и detached-владение.

## Flow: повторение цикла

Добавьте `RepeatCycleModuleFactory` в `AbilityDefinition.FlowModules`. `RepeatCount` — число дополнительных повторов после первого выполнения; `RepeatFrom` задаёт фазу возврата (`Targeting`, `Casting` или `Release`).

Если Flow-модулей нет, продолжают работать прежние поля `Release.Repeat` и `Release.RepeatFrom`. Если подключён Flow-модуль, выбор следующей фазы выполняет `AbilityFlowResolver`.

## Progression

У экземпляра `Ability` есть `ProgressionState`. Установите загруженный уровень до запуска:

```csharp
ability.ProgressionState.SetLevel(ability.Data, savedLevel);
```

Для повышения уровня:

```csharp
bool upgraded = ability.ProgressionState.TryUpgrade(ability.Data, activeContext);
```

`AbilityLevelDefinition` задаёт множители урона/перезарядки и дополнительные модули. Модули всех достигнутых уровней активируются при старте; при повышении уровня новые модули подключаются и публикуется `AbilityLevelChanged`.

`AbilityContext.Parameters` содержит стандартные `DamageMultiplier` и `CooldownMultiplier`, а также предоставляет `GetFloat/GetInt/GetBool` и `SetFloat/SetInt/SetBool` для проектных параметров. Контекст фиксирует значения на начало активации.

## Причина завершения

`TryCancel` и `TryInterrupt` публикуют разные события завершения. `Finish()` означает нормальное завершение. Событие причины передаётся модулям до их освобождения.

Реализация рассчитана на Unity 2022.3+. Перед использованием выполните компиляцию и Play Mode проверки в целевой версии Unity.
