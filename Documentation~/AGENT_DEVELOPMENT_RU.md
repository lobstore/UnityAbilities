# Руководство для агентов: проектирование и разработка новых способностей

Это руководство предназначено для coding agents, которым нужно **создавать или изменять игровые способности и их реализацию**, а не просто запускать уже существующие. Оно описывает архитектурный контракт Unity-пакета и порядок разработки.

Для агентов, которым нужно только выбирать, запускать, отменять или прерывать готовые способности, используйте отдельный [Agent API Reference](AGENT_API.md).

## 1. Изучите архитектуру перед изменениями

Пакет отделяет конфигурацию способности от runtime-поведения конкретной активации:

- `AbilityData` — основной ScriptableObject. Содержит имя способности, cooldown и ссылки на конфигурации каждой встроенной фазы.
- `AbilityStateStrategyConfig` и производные классы фаз — ScriptableObject-конфигурации. Хранят параметры, доступные разработчику/дизайнеру, и создают runtime-объекты стратегий.
- `IAbilityStateStrategy` — контракт runtime-фазы: `OnEnter`, `OnUpdate`, `OnExit`, `CanCancel`, `CanBeInterrupted`.
- `AbilityState` оборачивает runtime-стратегию. `AbilityStateMachine` управляет входом, обновлением, переходами и завершением состояний.
- `AbilityContext` передаёт исполнителя, текущую цель и точку, Animator, AbilityRuntime, точку применения и машину состояний.
- `AbilityController` — пример MonoBehaviour, запускающий настроенные способности и передающий события анимации. Это пример интеграции, а не обязательная зависимость любой игры.

Стандартный порядок фаз:

```text
Warmup -> Targeting -> Casting -> Release -> Finished
```

Стратегия может выполнить переход раньше или пропустить фазу, если этого требует дизайн способности.

## 2. Решите, нужна новая стратегия или новое базовое состояние

**Правило по умолчанию: добавляйте пару конфигурация/runtime-стратегия; не создавайте новое базовое состояние для каждой способности.**

Встроенные фазы покрывают основные типы поведения:

| Требуемое поведение | Предпочтительная точка расширения |
| --- | --- |
| Подготовительная анимация, звук, стартовый VFX | Warmup config/runtime |
| Выбор по курсору, на себя, ближайшая цель, конус целей | Targeting config/runtime |
| Каналирование, прогресс применения, ожидание события анимации | Casting config/runtime |
| Снаряд, удар ближнего боя, лечение, AoE, призыв, наложение баффа | Release config/runtime |

Создавайте новый класс `AbilityState` только тогда, когда проекту действительно нужна новая фаза жизненного цикла с отдельной семантикой переходов, общей для нескольких способностей. Это изменение ядра: потребуется обновить `AbilityData`, создание состояний и переходы, интеграцию контроллера, примеры, обе языковые версии документации и тесты. Не создавайте новый класс состояния ради разового эффекта одной способности.

## 3. Реализуйте стратегию фазы

Каждое поле `AbilityData` имеет тип конкретной фазы. Пользовательская конфигурация должна наследоваться от соответствующего класса, чтобы её можно было назначить в Inspector:

- Warmup: `WarmupStrategyConfig`
- Targeting: `TargetingStrategyConfig`
- Casting: `CastingStrategyConfig`
- Release: `ReleaseStrategyConfig`

Пользовательская конфигурация обычно добавляет сериализуемые параметры и переопределяет `CreateRuntime()`. Runtime-данные (прошедшее время, кэш цели, ссылки на созданные объекты, флаги animation events) храните в runtime-стратегии, а не в ScriptableObject. Каждый вызов `CreateRuntime()` должен создавать новый экземпляр стратегии для новой активации способности.

Все встроенные конфигурации фаз наследуют общие поля `CinemachinePrefab` и `ParentToCaster`. Укажите в `CinemachinePrefab` любой prefab с камерным rig: Cinemachine Virtual Camera, Cinemachine Camera, Dolly rig или собственную композицию. Камера создаётся при `OnEnter` соответствующей фазы и уничтожается при `OnExit`. При `ParentToCaster` prefab становится дочерним объектом исполнителя с сохранением локального смещения и поворота. Для отдельного ракурса наведения назначьте camera prefab в конфигурации Targeting. Runtime не зависит от конкретной версии Cinemachine; для динамической привязки Follow/LookAt или цели реализуйте `IAbilityCinematicCameraBinder`.

### Пример: собственная фаза Targeting

Конфигурация:

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

В примере используется базовый жизненный цикл Targeting, а изменён только поиск цели. Сохраняются стандартные таймер, политики отмены/прерывания, VFX/SFX, анимация и переход в Casting. При необходимости другого жизненного цикла переопределите подходящий virtual-метод или реализуйте `IAbilityStateStrategy` напрямую.

### Реализация Warmup или Casting с нуля

`WarmupStrategyRuntime` и `CastingStrategyRuntime` предоставляют virtual-методы жизненного цикла. При расширении этих классов вызывайте `base.OnEnter(ctx)` и `base.OnExit(ctx)`, чтобы сохранить создание и очистку общей камеры, а также стандартные эффекты и флаги Animator. Для полностью самостоятельного поведения можно вернуть runtime, реализующий `IAbilityStateStrategy` (и `IAnimationEventSink`, если нужны события анимации); такая реализация сама отвечает за принадлежащие ей ресурсы камеры.

### Реализация Release

Если Release отличается от стандартного поведения, наследуйте runtime от `ReleaseStrategyRuntime` и вызывайте `base.OnEnter(ctx)`, чтобы сохранить общие VFX/SFX/анимацию. При переопределении `OnExit(ctx)` вызывайте `base.OnExit(ctx)`, если намеренно не заменяете стандартную логику cooldown. Базовый `OnExit` Release запускает перезарядку способности.

## 4. Контракт жизненного цикла runtime

Продуманно реализуйте каждый метод стратегии:

| Метод | Ответственность |
| --- | --- |
| `OnEnter(ctx)` | Сброс runtime-полей, кэширование ссылок, запуск анимации/VFX/звука, создание настроенного camera prefab и подписка на необходимые события. |
| `OnUpdate(ctx)` | Обновление таймеров и игровой логики; запрос перехода, когда фаза завершена. |
| `OnExit(ctx)` | Сброс флагов Animator, отписка от событий, освобождение временных ресурсов и очистка объектов фазы, включая временную камеру. |
| `CanCancel(ctx)` | Проверка возможности отмены в данный момент. Метод не должен иметь побочных эффектов. |
| `CanBeInterrupted(ctx)` | Проверка возможности заменить способность другой в данной фазе. Метод не должен иметь побочных эффектов. |

Для перехода используйте `ctx.OwnerMachine.ChangeState(new CastingState(...))` или соответствующее следующее состояние. `ctx.OwnerMachine.Finish()` используйте при нормальном завершении способности или когда реализация намеренно требует безусловного завершения. Игровая отмена и замена способности должны проходить через API с проверкой политик.

### Отмена и прерывание для каждой фазы

Все встроенные конфигурации наследуют независимые параметры от `AbilityStateStrategyConfig`:

- `AllowCancel`: разрешает явный запрос отмены во время этой фазы.
- `MinTimeBeforeCancel`: минимальное время с момента входа в эту фазу, после которого отмена разрешена.
- `IsInterruptible`: разрешает заменить текущую способность другой во время этой фазы.

Встроенные runtime-стратегии используют эти настройки. Пользовательская реализация должна сохранять тот же контракт: `CanCancel` учитывает `AllowCancel` и `MinTimeBeforeCancel`, если дизайн явно не требует другого поведения; `CanBeInterrupted` учитывает `IsInterruptible`, если это намеренно не переопределено. Не смешивайте отмену и прерывание.

Успешная отмена или прерывание вызывает `OnExit` активной стратегии. Это останавливает машину состояний, но автоматически не отменяет созданные снаряды, урон, сетевые запросы и другие внешние эффекты. Определите, какие объекты принадлежат способности, и очищайте их в `OnExit`, если при отмене они должны исчезнуть. Эффекты, которые должны жить после Release, передавайте отдельному владельцу.

## 5. Правильно используйте AbilityContext

Доступные runtime-поля:

| Поле | Назначение |
| --- | --- |
| `Caster` | Персонаж/GameObject, применяющий способность. |
| `Target` | Выбранный GameObject цели; может быть null. |
| `TargetPoint` | Мировая точка назначения/попадания. |
| `Animator` | Animator персонажа; может быть null. |
| `Ability` | Текущий `AbilityRuntime`; данные доступны через `ctx.Ability.Data`. |
| `CastOrigin` | Transform точки запуска снаряда/удара; проверяйте перед обращением. |
| `OwnerMachine` | Машина состояний для переходов и штатного завершения. |

Не считайте, что цель всегда существует. Определите поведение для self-target, ground-target и отсутствующей цели. Проверяйте необязательные Unity-ссылки. Не используйте `Camera.main` без проверки наличия камеры; для детерминированного targeting или multiplayer лучше передавать ссылку на камеру явно.

## 6. События анимации

Runtime-стратегия, которой нужны animation events, должна реализовать `IAnimationEventSink`:

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

    // Реализуйте также OnUpdate, OnExit, CanCancel и CanBeInterrupted.
}
```

Пример `AbilityController` передаёт имена animation events активной стратегии, если она реализует этот интерфейс. Имя события в Animator Clip должно точно совпадать с ожидаемым. Сбрасывайте флаги событий в `OnEnter`, чтобы старое событие не повлияло на следующую активацию.

Встроенные конфигурации также содержат `ExitStateOnAnimationEvent` и `AnimationEventName`. Используйте их, когда достаточно стандартного перехода по времени или событию.

## 7. Соберите AbilityData asset

После добавления скриптов:

1. Создайте пользовательские конфигурации через пункты их `CreateAssetMenu`.
2. Создайте asset `AbilityData` через **Assets → Create → Abilities → Ability Data**.
3. Укажите `AbilityName` и `CooldownDuration`.
4. Назначьте все четыре ссылки на конфигурации фаз: Warmup, Targeting, Casting и Release. Для фаз без особого поведения используйте встроенные конфигурации.
5. Независимо настройте отмену и прерывание в каждой фазе.
6. Назначьте `AbilityData` в loadout/controller игровой системы.

Текущая модель `AbilityData` ожидает все четыре ссылки. Не оставляйте обязательную ссылку null, если одновременно не добавили явную null-safe логику переходов в framework.

## 8. Соглашения репозитория и расположение файлов

Следуйте текущей структуре примеров:

- Runtime стратегии: `Samples/Example/Scripts/Strategies/<Phase>/<AbilityName>Runtime.cs`
- ScriptableObject конфигурация: `Samples/Example/Scripts/Strategies/<Phase>/Factories/<AbilityName>Config.cs`
- Переиспользуемая логика пакета: `Runtime/Core/Strategies/` и `Runtime/Factories/` — только если поведение относится к общему framework, а не к одной sample/game ability.
- Документация: обновляйте `Documentation~/en/README.md` и `Documentation~/ru/README.md`, если меняется публичный workflow или настройка.

Используйте namespace `LOGIYGames.Abilities`, как в существующих примерах. При работе в локальной Unity-копии добавляйте `.meta` для новых Unity-файлов; пусть Unity генерирует корректные GUID, не копируйте GUID из других файлов.

## 9. Чек-лист проверки

Перед тем как сообщить о готовой способности:

- [ ] Описано ожидаемое поведение способности и ответственность каждой фазы.
- [ ] Каждая пользовательская конфигурация наследуется от правильного типа.
- [ ] Каждый `CreateRuntime()` возвращает новый runtime-экземпляр.
- [ ] Runtime-поля и флаги animation events сбрасываются при каждом `OnEnter`.
- [ ] Все четыре конфигурации фаз назначены в `AbilityData`.
- [ ] Отмена и прерывание независимо настроены для каждой фазы.
- [ ] Все методы `IAbilityStateStrategy` реализованы с определённым поведением.
- [ ] `OnExit` восстанавливает Animator flags и очищает принадлежащие фазе ресурсы.
- [ ] Определено поведение без цели и fallback target point.
- [ ] Проверены обычные переходы, отмена и прерывание.
- [ ] Проверено поведение cooldown. По умолчанию cooldown запускается при выходе из Release; отмена до Release сама по себе cooldown не запускает.
- [ ] Пакет компилируется в целевой версии Unity.
- [ ] Play Mode проверяет нормальный сценарий и сценарии отмены/прерывания.

Сейчас в репозитории есть sample project, но нет отдельного автоматизированного набора тестов. Не заявляйте об автоматической проверке или проверке в Unity, если она фактически не запускалась.

## 10. Что агент должен передать в результате

При запросе на реализацию способности предоставьте:

1. Краткое описание поведения и распределения логики по фазам.
2. Необходимые config и runtime scripts.
3. Новый или изменённый `AbilityData`/sample asset и соответствующие `.meta`, если среда поддерживает работу с Unity assets.
4. Обновлённую документацию на английском и русском, если меняются публичные настройки или поведение.
5. Результаты проверок с разделением выполненных и невыполненных проверок.

Не останавливайтесь на написании вызова контроллера, который запускает уже готовый `AbilityRuntime`: это интеграция/использование, а не реализация способности.
