# Ability Modules, Effects, Flow и Progression

Эта документация описывает расширения runtime API. Все модули создают отдельный runtime-экземпляр через `CreateRuntime`; изменяемое состояние не должно храниться в factory-конфигурации.

## Точки подключения

- `AbilityDefinition.Modules` — модули на всё время одной активации.
- `AbilityStateStrategyFactory.Modules` — модули конкретной фазы.
- `AbilityObjectStrategyFactory.Modules` — модули отдельного `AbilityObject`.
- `AbilityDefinition.FlowModules` — правила переходов между фазами.
- `AbilityDefinition.Progression` — конфигурация уровней навыка.

Списки используют `SerializeReference`. В Inspector добавляйте конкретные реализации из списка managed references.

## События

`AbilityEventDispatcher` публикует:

- `AbilityStarted`, `StateEntered`, `StateExited`;
- `AnimationEvent`, `IterationStarted`, `ReleasePerformed`;
- `AbilityCancelled`, `AbilityInterrupted`, `AbilityEnded`;
- `AbilityLevelChanged`, `ObjectCollision`.

Каждый модуль наследуется от `AbilityModuleRuntime` и может переопределить `OnEvent`, `Tick` и `Dispose`.

## Встроенные presentation-модули

### SpawnVFXModuleFactory

Настройте `Prefab`, `Trigger`, `FilterByPhase`/`Phase`, позицию, anchor, lifetime и `ResourceScope`.

### SpawnAttachedObjectModuleFactory

Создаёт объект при старте способности и прикрепляет его к персонажу или именованной дочерней точке. Например, укажите `AnchorName = RightHand`, prefab фантомного меча и `ResourceScope = Ability`: объект будет удалён при завершении способности.

### PlaySFXModuleFactory

Воспроизводит `AudioClip` при выбранном событии. При назначении `AudioSource` используется `PlayOneShot`; без источника создаётся временный AudioSource около кастера.

## Gameplay Effects

Добавьте `AbilityEffectController` на GameObject, который принимает эффекты. Он реализует `IAbilityEffectReceiver` и обновляет активные эффекты, снимая их по завершении длительности.

`ApplyEffectModuleFactory` выбирает кастера, первую цель или все цели. Модуль можно разместить в `AbilityDefinition.Modules`, чтобы он слушал события на протяжении всей способности, либо в фазовой конфигурации.

`AbilityEffectDefinition` — расширяемая ScriptableObject-конфигурация. Для собственных баффов/дебаффов унаследуйте её и реализуйте `CreateRuntime`; runtime эффекта реализует `OnApply`, `Tick`, `OnRemove`. Встроенный `TimedVisualEffectDefinition` показывает базовый эффект, создающий визуальный объект на цели.

Политики наложения: `Stack`, `Refresh`, `Ignore`.

## Владение ресурсами

`AbilityResourceRegistry` поддерживает:

- `Ability` — очищается при завершении способности;
- `State` — очищается при выходе из соответствующей фазы;
- `Detached` — не очищается вместе с родительской способностью.

Для ресурсов, которые создаёт модуль, регистрируйте `GameObject`, `IDisposable` или callback освобождения. Чтобы передать существующий handle, используйте `Resources.Detach(handle)`.

У снаряда, которому нужно пережить завершение каста, оставляйте собственный lifetime и scope `Detached`. Внутренние модули `AbilityObject` получают события столкновений только своего объекта.

## Flow: повторение цикла

Добавьте `RepeatCycleModuleFactory` в `AbilityDefinition.FlowModules`.

- `RepeatCount` — количество дополнительных повторов после первой итерации;
- `RepeatFrom` — фаза возврата: `Targeting`, `Casting` или `Release`.

Когда подключён Flow-модуль, `ReleaseStrategyRuntime` передаёт выбор следующей фазы `AbilityFlowResolver`. Если Flow-модулей нет, продолжают работать старые поля `Release.Repeat` и `Release.RepeatFrom` для обратной совместимости.

## Progression

У `Ability` есть собственный `ProgressionState`. Установить уровень до активации можно так:

```csharp
ability.ProgressionState.SetLevel(ability.Data, savedLevel);
```

Для повышения уровня:

```csharp
bool upgraded = ability.ProgressionState.TryUpgrade(ability.Data, activeContext);
```

Каждый `AbilityLevelDefinition` может задавать множители урона/перезарядки и список дополнительных модулей. Модули уровней до текущего включительно активируются при старте способности; при повышении уровня новые модули добавляются и публикуется `AbilityLevelChanged`.

`AbilityContext.Parameters` содержит `DamageMultiplier` и `CooldownMultiplier`, а также предоставляет типизированные методы `GetFloat/GetInt/GetBool` и `SetFloat/SetInt/SetBool` для проектных параметров. Снимок параметров создаётся при старте runtime контекста.

## Конец способности

`AbilityStateMachine.TryCancel` и `TryInterrupt` публикуют разные причины завершения. `Finish()` означает нормальное завершение. Модули получают событие причины завершения до их освобождения.

Реализация рассчитана на Unity 2022.3+. Перед использованием в проекте выполните проверку компиляции и Play Mode тесты в целевой версии Unity.
