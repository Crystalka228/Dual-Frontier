# Шины событий

Шина событий — центральный механизм взаимодействия систем. Прямые вызовы между системами запрещены сторожем изоляции, так что все горизонтальные связи идут через шины. Правильный выбор модели доставки и типа шины напрямую определяет производительность и корректность многопоточного кода.

## Почему доменные шины, а не глобальная

Одна шина на всё — естественное решение, пока систем десяток. На сотне систем и десяти тысячах entity она превращается в узкое горло: `ConcurrentDictionary` с подписчиками всех типов разом становится точкой блокировки. Профилирование показывает один и тот же паттерн: 70% времени шины тратится на contention в одном словаре.

Dual Frontier делит шину по игровым доменам. Каждая шина — это самостоятельный `DomainEventBus` со своей таблицей подписчиков и своим пулом буферов для отложенной доставки.

```csharp
public interface IGameServices
{
    ICombatBus    Combat    { get; }
    IInventoryBus Inventory { get; }
    IMagicBus     Magic     { get; }
    IWorldBus     World     { get; }
    IPawnBus      Pawns     { get; }
}
```

Преимущества:

- Меньше contention: `CombatSystem` пишет только в `Combat`, `InventorySystem` — только в `Inventory`, их блокировки не пересекаются.
- Проще дебажить: лог выводится по конкретной шине.
- Легче профилировать: `dotTrace` показывает время каждой шины отдельно.
- Явная декларация: атрибут `[SystemAccess(bus: nameof(IGameServices.Combat))]` задаёт, куда система вообще имеет право писать.

## Синхронная, Deferred, Immediate

Шина поддерживает три режима доставки. Режим задаётся атрибутом на классе события.

### Синхронная (по умолчанию)

Обработчики вызываются прямо внутри `Publish`, до возврата управления. Все подписчики успевают обработать событие в текущей фазе планировщика. Простая модель, но обработчик не должен блокироваться — он тормозит всю фазу.

### [Deferred]

Событие кладётся в per-`DomainEventBus` очередь и доставляется
на границе между фазами планировщика. Используется, когда порядок имеет
значение: `DeathEvent` всегда помечается `[Deferred]`, чтобы к моменту
доставки все параллельные системы уже закончили чтение `HealthComponent`.

```csharp
[Deferred]
public sealed record DeathEvent : IEvent
{
    public required EntityId EntityId { get; init; }
    public required EntityId? KillerId { get; init; }
}
```

#### Механика доставки (v0.3)

`Subscribe<T>(Action<T>)` захватывает текущий `SystemExecutionContext.Current` —
обычно это контекст системы, чей `OnInitialize` сейчас выполняется. Захват
сохраняется вместе с обработчиком.

`Publish<T>(T)` для `[Deferred]` enqueue-ит `(eventType, evt)` в очередь
шины. Очередь — `ConcurrentQueue`, безопасна для параллельной публикации
из систем одной фазы.

`ParallelSystemScheduler.ExecutePhase` после барьера `Parallel.ForEach`
вызывает внутренний `IDeferredFlush.FlushDeferred()` на агрегаторе шин
(`GameServices`). Тот снимает snapshot текущей очереди каждой шины, очищает,
и для каждого подписчика делает:

1. Push'ит **захваченный при Subscribe** контекст подписчика.
2. Вызывает обработчик.
3. Pop'ит контекст в `finally`.

Это даёт изоляционно-корректную мутацию компонентов из обработчика:
`SetComponent<T>` проверяет `_allowedWrites` контекста подписчика, а не
публикующей системы. Поэтому Phase 4 `HaulSystem` (writes=`[]`) может
безопасно публиковать `[Deferred] ItemRemovedEvent`, а `InventorySystem`
(writes=`[StorageComponent]`) применит мутацию из своего контекста в
следующей фазе.

Re-entrancy: `[Deferred]` событие, опубликованное **внутри** другого
`[Deferred]`-обработчика, попадает в очередь и доставится на следующем
вызове `FlushDeferred` (т.е. на следующей фазе) — текущий drain работает
со снимком и не подбирает новопоступивших.

### [Immediate]

Крайне редкий режим: событие гарантированно доставляется синхронно из
`Publish` и **никогда** не попадает в очередь `[Deferred]`. Используется
для критических срывов — например, `EtherSurgeCriticalEvent` на высоком
уровне, когда дальнейшая параллельная работа бессмысленна.

```csharp
[Immediate]
public sealed record EtherSurgeCriticalEvent : IEvent
{
    public required EntityId CasterId { get; init; }
}
```

В текущей реализации `[Immediate]` функционально эквивалентен sync-доставке
по умолчанию (так как фазы и так сериализуют публикацию-обработку), но
атрибут несёт декларативное намерение и явно исключает попадание события в
deferred-очередь — что важно для UI-критических сигналов и для будущих
оптимизаций шины.

## Двухшаговая модель Intent → Granted/Refused

Синхронный `Request/Response` в многопоточной среде — ловушка: либо блокирующий вызов (убивает параллелизм), либо сложная синхронизация с `TaskCompletionSource` (ломает декларацию доступа и сторож). Dual Frontier использует намерения и батч-разрешение.

```csharp
// ОПАСНО — блокирует поток в многопоточной фазе.
var result = bus.Request<AmmoRequest, AmmoResult>(...);

// БЕЗОПАСНО — двухшаговая модель.

// ШАГ 1 (фаза сбора намерений):
// CombatSystem публикует намерение — не блокируется.
bus.Combat.Publish(new AmmoIntent {
    RequesterId = entityId,
    AmmoType    = weapon.RequiredAmmo,
    Position    = position
});
```

В следующей фазе другая система собирает все `AmmoIntent` разом и отвечает батчем `AmmoGranted`/`AmmoRefused`. Запросчик получает ответ в ещё одной фазе и уже в ней решает, что делать.

## Батч-обработка на примере AmmoIntent

`InventorySystem` получает список всех `AmmoIntent` за фазу и обходит кэш патронов один раз — вместо отдельного скана на каждое намерение.

```csharp
void OnAmmoIntentBatch(IReadOnlyList<AmmoIntent> intents)
{
    foreach (var intent in intents)
    {
        var granted = TryReserveFromCache(intent);
        bus.Inventory.Publish(granted
            ? new AmmoGranted { RequesterId = intent.RequesterId }
            : new AmmoRefused { RequesterId = intent.RequesterId });
    }
}
```

Батчинг = один проход по кэшу вместо N отдельных запросов. Для 100 пешек выигрыш двухзначный: вместо 100 сканов по 60 раз в секунду — один проход раз в три фрейма. Цифры из целевых метрик: `≤100 сканов/сек` против `6000 сканов/сек` в RimWorld.

`IntentBatcher` в `DualFrontier.Core/Bus` отвечает за сбор намерений: системы-обработчики подписываются не на отдельное событие, а на батч.

## Модель Lease (v02 §12.1)

Двухшаговая модель `Intent → Granted/Refused` рассчитана на дискретные запросы: один патрон, один заряд маны. Когда ресурс расходуется не один раз, а каждый тик (канал заклинания, поддержка щита, активный ritual) — цепочка Intent'ов каждый тик избыточна и плохо переживает срывы. Для таких сценариев v0.2 вводит модель **Lease** (аренда).

### Жизненный цикл

```
Open → Active (drain per tick) → Closed
```

- **Open.** Инициатор публикует `ManaLeaseOpenRequest` с `DrainPerTick`, `MinDurationTicks` и `MaxDurationTicks`. `ManaLeaseRegistry` проверяет запас, резервирует `MinDurationTicks * DrainPerTick` (reserve-then-consume) и отвечает `ManaLeaseOpened` с `LeaseId`. При нехватке — `ManaLeaseRefused` с `RefusalReason`.
- **Active.** Каждый тик `ManaSystem` дренит `DrainPerTick` из зарезервированного запаса. При истощении резерва lease либо продлевается (если мана есть), либо закрывается.
- **Closed.** Явное закрытие через `ManaLeaseClosed` (`[Deferred]`) с `CloseReason` (`OwnerRequested`, `MaxDurationReached`, `Starvation`, `OwnerDied`). Остаток резерва возвращается в `ManaComponent`.

Reserve-then-consume гарантирует, что lease не сорвётся на первом же тике из-за параллельного намерения, съевшего ману между Open и первым дреном.

### Новые события

- `ManaLeaseOpenRequest` — запрос открытия.
- `ManaLeaseOpened` — подтверждение с `LeaseId`.
- `ManaLeaseRefused` — отказ с `RefusalReason`.
- `ManaLeaseClosed` (`[Deferred]`) — закрытие с `CloseReason`.

Аналогичные события существуют для щитов, ритуалов и любых других Lease-подобных ресурсов — имена формируются по тому же шаблону `{Resource}Lease{Stage}`.

Подробности выбора между Intent и Lease и правила применения описаны в [RESOURCE_MODELS](./RESOURCE_MODELS.md).

## Lifecycle подписок

Подписка на шину требует отписки: обработчик держит ссылку на подписчика, и без `Unsubscribe` в `Dispose` возникает утечка памяти. Система вызывает `Subscribe` в своём `OnInitialize()` и `Unsubscribe` в `OnDispose()`. Планировщик гарантирует вызов обоих методов.

```csharp
protected override void OnInitialize()
{
    Bus.Combat.Subscribe<AmmoGranted>(OnAmmoGranted);
    Bus.Combat.Subscribe<AmmoRefused>(OnAmmoRefused);
}

protected override void OnDispose()
{
    Bus.Combat.Unsubscribe<AmmoGranted>(OnAmmoGranted);
    Bus.Combat.Unsubscribe<AmmoRefused>(OnAmmoRefused);
}
```

Мод подписывается и отписывается через `IModApi` — реализация сама отслеживает подписки и снимает их при выгрузке `AssemblyLoadContext`. Моду не нужно помнить, что именно он подписал.

## Профилирование

Каждая шина ведёт счётчики: число публикаций, число доставок, среднее время обработки на событие, пиковый размер батча. Счётчики экспортируются в `BenchmarkRegistry` и отображаются в диагностическом оверлее F3.

Типичные симптомы:

- **Высокий PublishCount, низкий SubscriberCount** — событие публикуется, но никто не слушает. Вероятно ошибка именования или устаревший код.
- **Пиковый batch > 1000** — слишком много намерений за фазу, нужно либо реже тикать инициатор, либо раньше фильтровать.
- **Deferred queue не пустеет к концу кадра** — подписчик блокируется. Искать `lock` или `await` в обработчиках (async запрещён, см. [THREADING](./THREADING.md)).

Для bottleneck-анализа подключается dotTrace timeline — каждая шина помечена отдельным markers, что позволяет видеть распределение нагрузки по доменам во времени.

## См. также

- [THREADING](./THREADING.md)
- [CONTRACTS](./CONTRACTS.md)
- [PERFORMANCE](./PERFORMANCE.md)
