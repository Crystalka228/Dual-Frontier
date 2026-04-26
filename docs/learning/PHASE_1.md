# Учебный материал: C# и многопоточность для Dual Frontier (Phase 1)

*Артефакт self-teaching ритуала после закрытия Phase 1 (Core ECS).
Полный курс, привязанный к реальным файлам проекта. Версия 1.0, 2026-04-25.*

*Каждая тема — реальный файл `src/`, не абстрактный пример.
Формат: 20–30 мин теории + 30–60 мин работа с кодом + практическая проверка.*

*Этот документ — referenced from [METHODOLOGY.md §4.5](./METHODOLOGY.md) как
эмпирическое подтверждение self-teaching ритуала между фазами.*

---

# 1. Архитектурный фундамент проекта

Прежде чем учить язык — нужно понять, как устроен проект. Dual Frontier состоит из строго разделённых слоёв. Нарушить зависимость между ними — архитектурная ошибка.

## 1.1 Четыре слоя (docs/ARCHITECTURE.md)

| Слой | Сборка | Что делает | Правило |
|---|---|---|---|
| Contracts | DualFrontier.Contracts | Интерфейсы, атрибуты, EntityId, GridVector. Никакой реализации. | Только BCL-зависимости |
| Core (Infrastructure) | DualFrontier.Core | World, ComponentStore, DependencyGraph, ParallelSystemScheduler, DomainEventBus. Всё internal. | internal-first, открыт Systems через InternalsVisibleTo |
| Domain | DualFrontier.Systems, .Components, .Events, .AI | Вся игровая логика. Системы, компоненты, события. | Не знает о Godot, многопоточно |
| Presentation / Application | DualFrontier.Presentation, .Application | Godot SceneTree, UI, GameLoop, SaveSystem, PresentationBridge. | Только main thread у Presentation |

> **📌 Правило** — Каждый слой знает только о слоях ниже себя. AI → Contracts + Components (не Core). Presentation → Application → Core. Нарушение = ошибка архревью.

## 1.2 Направление зависимостей

```csharp
Contracts ← Components ← Systems ← Application ← Presentation
Contracts ← Events     ↗
Contracts ← Core       ↗  (Core открыт Systems через InternalsVisibleTo)
Contracts ← AI (зависит только от Contracts + Components)
```

> **⚠ Запрет** — Мод видит ТОЛЬКО DualFrontier.Contracts — через AssemblyLoadContext. Прямая ссылка мода на Core физически заблокирована. Это защита, а не договорённость.

# 2. C# как язык контрактов

## 2.1 class vs struct — где что применяется

В Dual Frontier это не академический вопрос — это архитектурное решение.

| Тип | Где в проекте | Почему | Ключевое правило |
|---|---|---|---|
| struct | EntityId, LeaseId, TransactionId, GridVector | Маленький, неизменяемый идентификатор. Копируется по значению — нет риска случайного шаринга. | readonly record struct — два int поля максимум |
| class | World, SystemBase, DomainEventBus, ComponentStore<T> | Объект состояния или сервис. Передаётся по ссылке — один экземпляр у всех. | sealed везде, где нет наследования |
| interface | IComponent, IEvent, IModApi, IGameServices, ICombatBus | Контракт формы взаимодействия без реализации. Всё модинг-API — интерфейсы. | Никаких методов в маркер-интерфейсах |

### EntityId — эталонный пример struct

```csharp
public readonly record struct EntityId(int Index, int Version)
{
    public static readonly EntityId Invalid = default;
    public bool IsValid => Index > 0;
}
```

> **💡 Понять** — EntityId.Version инкрементируется при удалении entity. Старая ссылка с version=1 на entity с version=2 — мёртвая ссылка. Система просто пропускает её, не крашится.

### Ссылка vs копия — ловушка со struct

Это самая частая ошибка при работе со struct. Запомни два случая:

```csharp
// ✅ ПРАВИЛЬНО — работаем с оригиналом через GetComponent/SetComponent
var hp = GetComponent<HealthComponent>(id);
hp.Current -= damage;
SetComponent(id, hp);   // записали изменение
```

```csharp
// ❌ ЛОВУШКА — если HealthComponent это struct
ref var hp2 = ref store.GetRef(id);  // нужна ref-семантика
hp2.Current -= damage;  // иначе меняем КОПИЮ, оригинал не изменился
```

> **⚠ Правило** — Если тип — class, изменение поля через ссылку меняет оригинал. Если struct — GetComponent возвращает КОПИЮ. Изменения нужно SetComponent обратно записать.

## 2.2 Generics — как читать сигнатуры

В проекте generics везде. Нужно читать их без страха.

| Сигнатура | Что означает | Где в проекте |
|---|---|---|
| ComponentStore<T> where T : IComponent | T — любой тип, реализующий IComponent. Одна реализация Store для всех компонентов. | DualFrontier.Core/ECS/ComponentStore.cs |
| void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent | Подписка на конкретный тип события. Компилятор гарантирует типобезопасность. | DomainEventBus, IEventBus |
| IEnumerable<EntityId> Query<T1, T2>() where T1, T2 : IComponent | Ленивый итератор по entity, у которых есть оба компонента. | SystemExecutionContext, SystemBase |
| GetComponent<T>(EntityId id) where T : IComponent | Получить компонент строго типизированно. Ошибка типа — на этапе компиляции. | SystemBase (через контекст) |

### Как работает ComponentStore<T> внутри

```csharp
internal sealed class ComponentStore<T> : IComponentStore where T : IComponent
{
    private int[]  _sparse;       // indexed by EntityId.Index → позиция в dense
    private T[]    _dense;         // плотный массив компонентов
    private int[]  _denseToIndex;  // dense[i] принадлежит entity с index denseToIndex[i]
}
```

IComponentStore — маркер-интерфейс без методов. Нужен только чтобы хранить разные ComponentStore<T> в одной коллекции Dictionary<Type, IComponentStore>.

## 2.3 Интерфейсы — контракты проекта

Интерфейс в Dual Frontier — это не просто C#-концепция. Это архитектурное обещание. Мод знает только об интерфейсах, не о реализации.

| Интерфейс | Смысл | Кто реализует |
|---|---|---|
| IComponent | Маркер: это чистые данные ECS. Никакой логики. | HealthComponent, PositionComponent, WeaponComponent... |
| IEvent | Маркер: это событие доменной шины. Неизменяемый record. | DamageEvent, AmmoIntent, ManaGranted... |
| IModApi | Контракт API для мода: RegisterComponent, RegisterSystem, Subscribe, Publish. | RestrictedModApi в Application |
| IGameServices | Агрегатор 5 шин: Combat, Inventory, Magic, Pawns, World. | GameServices в Core |
| ICombatBus, IMagicBus... | Контракт конкретной доменной шины. | CombatBus, MagicBus внутри Core |

> **💡 Принцип** — Мод регистрирует SystemBase-потомка через IModApi.RegisterSystem<T>(). Не знает о World, не знает о Scheduler. Знает только что его система будет вызвана с delta.

## 2.4 Атрибуты — декларативный язык системы

Атрибуты в Dual Frontier — это не метаданные для читателя кода. Это декларации, которые DependencyGraph читает через Reflection при старте.

```csharp
[SystemAccess(
    reads:  new[] { typeof(PositionComponent), typeof(WeaponComponent) },
    writes: new[] { typeof(HealthComponent) },
    bus:    nameof(IGameServices.Combat)   // не строка — nameof
)]
[TickRate(TickRates.FAST)]
public sealed class CombatSystem : SystemBase { ... }
```

| Атрибут | Что читает | Зачем |
|---|---|---|
| [SystemAccess] | DependencyGraph.AddSystem() | Строит граф READ/WRITE конфликтов. Определяет порядок фаз. |
| [TickRate] | TickScheduler.ShouldRun() | REALTIME=60Hz, FAST=30Hz, NORMAL=10Hz, SLOW=2Hz, RARE=0.2Hz |
| [Deferred] | DomainEventBus.Publish() | Событие доставляется в следующей фазе, не мгновенно. |
| [Immediate] | DomainEventBus.Publish() | Критическое прерывание — доставка раньше остальных. |

### Как читать атрибуты через Reflection

```csharp
SystemAccessAttribute? access =
    systemType.GetCustomAttribute<SystemAccessAttribute>(inherit: false);
if (access is null)
    throw new InvalidOperationException($"System '{systemType.Name}' has no [SystemAccess]");
```

inherit: false — важный параметр. Атрибут ищется только на конкретном типе, не у родителей. Это гарантирует что каждая система явно декларирует свой доступ.

## 2.5 Nullable и исключения

Nullable-контекст должен быть включён. Это ранняя защита контракта — null на входе = нарушение.

```csharp
// SystemExecutionContext — nullable везде правильно
public static SystemExecutionContext? Current => _current.Value;
// null = поток не принадлежит планировщику (Godot main thread, тест без контекста)
```

```csharp
// SystemBase использует null-check как guard
var ctx = SystemExecutionContext.Current
    ?? throw new InvalidOperationException(
        "GetComponent called outside an active scheduler context.");
```

> **📌 Правило finally** — PopContext обязан быть в finally. Если Update() бросит исключение — поток всё равно освободит контекст. Иначе следующая система на том же потоке получит nested push и упадёт с диагностикой о баге в планировщике.

```csharp
SystemExecutionContext.PushContext(ctx);
try {
    system.Update(delta);
}
finally {
    SystemExecutionContext.PopContext();  // всегда, даже при исключении
}
```

# 3. Коллекции — часть модели производительности

В проекте коллекция — это не просто контейнер. Выбор типа коллекции влияет на производительность симулятора с 10-20 тысячами entity.

| Коллекция | Сложность | Где в проекте | Когда использовать |
|---|---|---|---|
| List<T> | O(1) append, O(n) search | Фазы SystemPhase.Systems, наборы систем, временные батчи | Нужен порядок, быстрый обход, lookup не нужен |
| Dictionary<TKey,TValue> | O(1) lookup | _stores в World (Type→IComponentStore), _contextCache в Scheduler | Быстрый lookup по ключу. Порядок не важен. |
| HashSet<T> | O(1) contains | _allowedReads, _allowedWrites в SystemExecutionContext | Проверка принадлежности, уникальность, пересечения |
| ConcurrentDictionary<K,V> | O(1) потокобез. | _stores в World, _handlers в DomainEventBus | Доступ из нескольких потоков. Не заменяет архитектуру. |
| int[] (SparseSet) | O(1) все операции | _sparse, _dense в ComponentStore<T> | Максимальная производительность ECS, cache-friendly |

### SparseSet — детали реализации

ComponentStore<T> использует SparseSet. Понять его структуру важно для дебага.

```csharp
private int[] _sparse;       // длина = maxEntityCount. sparse[EntityId.Index] → позиция в dense или -1
private T[]   _dense;         // плотный массив: dense[0], dense[1]... без дыр
private int[] _denseToIndex;  // параллельно dense: какой EntityId.Index в этом слоте
```

| Операция | Как работает | Сложность |
|---|---|---|
| Add(id, component) | sparse[id.Index] = Count; dense[Count] = component; Count++ | O(1) |
| Remove(id) | Swap last element into removed slot. sparse[last.Index] = denseIdx. Count--. | O(1) через swap-with-last |
| Get(id) | return dense[sparse[id.Index]] | O(1) |
| Iterate all | Пробег по dense[0..Count-1] — плотный, cache-friendly | O(Count), без пропусков |
| Query<T1,T2> | Берём меньший store, проверяем наличие T2 для каждого entity | O(min(N1,N2)) |

### Правило выбора коллекции (три вопроса)

- 1. Нужен ли порядок? → Если да: List или sorted структура
- 2. Нужен ли быстрый lookup по ключу? → Dictionary или HashSet
- 3. Будет ли доступ из нескольких потоков? → ConcurrentDictionary или отдельная синхронизация

> **⚡ Важно** — ConcurrentDictionary делает отдельные операции атомарными, но НЕ делает последовательность операций (read-modify-write) атомарной. Это не замена правильной фазовой модели.

# 4. ECS-ядро: World, Entity, Component, System

## 4.1 Жизненный цикл Entity

```csharp
// 1. Создание
EntityId id = world.CreateEntity();
// id.Index = уникальный индекс, id.Version = 1
```

```csharp
// 2. Добавление компонента
world.AddComponent(id, new HealthComponent { Current = 100, Maximum = 100 });
```

```csharp
// 3. Доступ через систему (не напрямую к World!)
// В Update() системы:
var hp = GetComponent<HealthComponent>(id);   // через SystemExecutionContext
```

```csharp
// 4. Удаление — инкрементирует Version
world.DestroyEntity(id);
// id.Version = 1, world.GetVersion(id.Index) = 2
// Теперь старый id — мёртвая ссылка
```

> **✅ Мёртвые ссылки** — После DestroyEntity старый EntityId безопасно невалиден — его Version не совпадёт с текущей. Системы проверяют IsValid и пропускают мёртвые entity вместо краша.

## 4.2 Изоляция системы — главный инвариант

Система в Dual Frontier НЕ обращается к World напрямую. Только через SystemExecutionContext, который проверяет декларацию [SystemAccess].

| Метод | Что проверяет контекст (DEBUG) | Ошибка при нарушении |
|---|---|---|
| GetComponent<T>(id) | T должен быть в reads ИЛИ writes атрибута | IsolationViolationException: UndeclaredRead |
| SetComponent<T>(id, v) | T должен быть в writes атрибута | IsolationViolationException: UndeclaredWrite |
| Query<T>() | T должен быть в reads или writes | IsolationViolationException: UndeclaredRead |
| GetSystem<TSystem>() | Всегда запрещено. Используй шину. | IsolationViolationException: DirectSystemAccess |
| Publish<TEvent>(evt) | Имя шины должно быть в allowedBuses | IsolationViolationException: UnauthorizedBus |

```csharp
// ✅ Правильная система
[SystemAccess(reads: new[]{ typeof(HealthComponent) }, writes: new Type[0], bus: nameof(IGameServices.Combat))]
[TickRate(TickRates.FAST)]
public sealed class DamageReporterSystem : SystemBase
{
    public override void Update(float delta)
    {
        foreach (var entity in Query<HealthComponent>())
        {
            var hp = GetComponent<HealthComponent>(entity);
            if (hp.Current <= 0) Services.Combat.Publish(new DeathEvent(entity));
        }
    }
}
```

> **⚠ Нарушение изоляции Core-системы** — Краш немедленно — IsolationViolationException. Нарушение — баг разработчика. В RELEASE-сборке проверки отключены (#if DEBUG), накладных расходов нет.

> **📌 Нарушение изоляции мод-системы** — Не краш, а маршрут через IModFaultSink → ModFaultHandler в Application. Мод выгружается, игра продолжает работать.

# 5. Многопоточность симулятора

## 5.1 DependencyGraph — как строятся фазы

DependencyGraph читает [SystemAccess] всех систем и строит граф конфликтов. W/W-конфликт на одном компоненте — ошибка при Build(). W/R-конфликт — ребро зависимости.

```csharp
// Пример конфликтов:
// WriterASystem    writes: CompA
// ReadAWriteB      reads: CompA, writes: CompB   → зависит от WriterA
// ReaderBSystem    reads: CompB                  → зависит от ReadAWriteB
```

```csharp
// Результат топологической сортировки (алгоритм Кана):
// Фаза 0: [WriterASystem]          — нет зависимостей
// Фаза 1: [ReadAWriteB]             — ждёт Фазу 0
// Фаза 2: [ReaderBSystem]           — ждёт Фазу 1
```

| Тип конфликта | Что происходит | Результат |
|---|---|---|
| W/W (оба пишут CompA) | Два потока пишут в один ComponentStore → гонка → битый мир | Build() бросает InvalidOperationException: Write conflict |
| W/R (один пишет, другой читает) | Читатель должен стартовать ПОСЛЕ писателя | Ребро в графе → разные фазы |
| R/R (оба читают) | Безопасно — можно в одной фазе | В одной фазе, параллельно |
| Цикл в графе | A зависит от B, B зависит от A — невозможно упорядочить | Build() бросает: Cyclic dependency |

## 5.2 Parallel.ForEach — как исполняются фазы

```csharp
// ParallelSystemScheduler.ExecutePhase()
Parallel.ForEach(phase.Systems, _parallelOptions, system =>
{
    if (!_ticks.ShouldRun(system)) return;
```

```csharp
    SystemExecutionContext ctx = _contextCache[system];
    SystemExecutionContext.PushContext(ctx);
    try { system.Update(delta); }
    finally { SystemExecutionContext.PopContext(); }
});
// Parallel.ForEach блокирует до завершения всех систем фазы
// Это и есть БАРЬЕР между фазами
```

| Деталь | Значение |
|---|---|
| MaxDegreeOfParallelism | Environment.ProcessorCount - 2. Резервирует ядро для Godot main thread и ОС. |
| Порядок внутри фазы | НЕ гарантирован. Системы одной фазы параллельны — не делай предположений. |
| Барьер между фазами | Parallel.ForEach блокирует ExecutePhase до завершения всех. Следующая фаза стартует только после. |
| Кэш контекстов | _contextCache строится один раз в конструкторе. Горячий путь — без Reflection, без аллокаций. |

## 5.3 ThreadLocal — почему guard привязан к потоку

```csharp
private static readonly ThreadLocal<SystemExecutionContext?> _current = new();
```

ThreadLocal<T> даёт каждому потоку независимое хранилище. Поток пула из Parallel.ForEach имеет свой _current. Godot main thread — свой (null, не в контексте планировщика).

| Поток | Значение _current | Что означает |
|---|---|---|
| Поток планировщика во время Update() | SystemExecutionContext конкретной системы | GetComponent/SetComponent разрешены согласно декларации |
| Поток планировщика вне Update() | null (после PopContext) | Доступ к компонентам запрещён |
| Godot main thread | null | Domain-код здесь не исполняется |
| Тест без PushContext | null | Тест должен явно PushContext для проверки guard-а |

> **🚫 async/await в системах — запрещено** — await переносит продолжение на другой поток. _current.Value на новом потоке = null. GetComponent() бросит исключение. Детерминизм нарушен. Фазовая семантика сломана. async/await в Domain — строго запрещено.

## 5.4 Race condition — как распознать

Гонка данных = результат зависит от порядка одновременного доступа потоков. В симуляторе это смертельно: ошибка проявится через часы игры.

| Сценарий | Почему опасно | Защита в проекте |
|---|---|---|
| Два потока пишут один ComponentStore | Данные перетираются непредсказуемо | DependencyGraph запрещает W/W в одной фазе |
| Один пишет, другой читает в одной фазе | Читатель видит частично обновлённое состояние | W/R тоже разносится по фазам |
| Мод регистрирует систему во время тика | Нарушение инварианта _phases при чтении в Parallel.ForEach | Rebuild() только из меню, не во время сессии |
| Обработчик шины подписывается во время Publish | ConcurrentModificationException | DomainEventBus копирует список перед итерацией |

> **✅ Критический принцип** — Для многопоточности в симуляторе лучше ранний краш, чем тихий баг. Если нарушен инвариант и миру нельзя доверять — игра должна остановиться до того, как испорченное состояние попадёт в save.

# 6. Event Bus — двухшаговая модель

## 6.1 Архитектура шин

В Dual Frontier 5 доменных шин: Combat, Inventory, Magic, Pawns, World. Каждая — отдельный экземпляр DomainEventBus. Это снижает lock contention и упрощает профилирование.

```csharp
// GameServices — агрегатор шин (docs/CONTRACTS.md)
public interface IGameServices
{
    ICombatBus    Combat    { get; }
    IInventoryBus Inventory { get; }
    IMagicBus     Magic     { get; }
    IPawnBus      Pawns     { get; }
    IWorldBus     World     { get; }
}
```

### Как работает DomainEventBus.Publish()

```csharp
// 1. Получаем список обработчиков для типа события
if (!_handlers.TryGetValue(eventType, out var handlersList)) return;
```

```csharp
// 2. Копируем список под lock — защита от изменений во время итерации
List<Delegate> handlersCopy;
lock (handlersList) { handlersCopy = new List<Delegate>(handlersList); }
```

```csharp
// 3. Вызываем обработчики вне lock — нет deadlock при Subscribe в handler
foreach (var handler in handlersCopy)
    ((Action<TEvent>)handler)?.Invoke(evt);
```

## 6.2 Intent → Granted/Refused (двухшаговая модель)

Ключевой паттерн для механик с ресурсами. Система не получает ресурс напрямую — она публикует намерение. Другая система отвечает в следующей фазе.

| Шаг | Кто | Что публикует | Фаза |
|---|---|---|---|
| 1 | CombatSystem | AmmoIntent { RequesterId, AmmoType, Position } | Фаза N |
| 2 | IntentBatcher | Собирает все AmmoIntent за фазу N | Фаза N |
| 3 | InventorySystem | Flush AmmoIntent → AmmoGranted или AmmoRefused | Фаза N+1 |
| 4 | CombatSystem | Подписан на AmmoGranted — исполняет выстрел | Фаза N+1 |

```csharp
// CombatSystem — публикует намерение (не блокирует)
Services.Combat.Publish(new AmmoIntent { RequesterId = pawnId, AmmoType = AmmoType.Rifle });
```

```csharp
// InventorySystem — обрабатывает пачку Intent в следующей фазе
var intents = _batcher.Flush<AmmoIntent>();
foreach (var intent in intents) {
    if (HasAmmo(intent.RequesterId, intent.AmmoType))
        Services.Combat.Publish(new AmmoGranted(intent.RequesterId));
    else
        Services.Combat.Publish(new AmmoRefused(intent.RequesterId));
}
```

> **💡 Принцип** — Двухшаговая модель исключает блокирующий request/response между системами. Каждая система остаётся независимой. Планировщик выстраивает их через граф зависимостей автоматически.

# 7. Дебаг — навыки для проекта

## 7.1 Читать stack trace

Stack trace читается снизу вверх: сначала найди первый throw (источник), не место где исключение всплыло наружу.

```csharp
System.InvalidOperationException: SystemExecutionContext is already set on this thread
   at DualFrontier.Core.ECS.SystemExecutionContext.PushContext(...)  ← ИСТОЧНИК
   at DualFrontier.Core.Scheduling.ParallelSystemScheduler.ExecutePhase(...)
   at DualFrontier.Core.Scheduling.ParallelSystemScheduler.ExecuteTick(...)
```

Этот краш означает: попытка вложенного PushContext — уже установленный контекст не был снят. Причина: PopContext не был в finally, и предыдущий Update() бросил исключение.

## 7.2 Типы нарушений изоляции

| Исключение / сообщение | Что нарушено | Как исправить |
|---|---|---|
| UndeclaredRead: T not in reads | Система читает компонент, не задекларированный в [SystemAccess] | Добавить T в reads: массив атрибута |
| UndeclaredWrite: T not in writes | Система пишет компонент без декларации | Добавить T в writes: массив атрибута |
| DirectSystemAccess | Система вызвала GetSystem<T>() — прямая ссылка на другую систему | Заменить на Publish через шину |
| UnauthorizedBus | Система публикует в шину, не задекларированную в bus: | Поправить bus: nameof(IGameServices.X) |
| nested push detected | PushContext без предшествующего PopContext | Убедиться что PopContext в finally |
| Write conflict in Build() | Две системы пишут один компонент — W/W конфликт | Разделить логику или объединить системы |
| Cyclic dependency in Build() | Цикл зависимостей A→B→A | Пересмотреть reads/writes, разбить через события |

## 7.3 Логическая vs архитектурная ошибка

| Тип ошибки | Признак | Где искать |
|---|---|---|
| Логическая | Неверный расчёт урона, не та формула, ошибка в if-условии | В самом Update() системы, в компонентах |
| Архитектурная | Исключение изоляции, W/W конфликт, краш планировщика, нарушение контракта | В атрибутах [SystemAccess], в фазовой модели, в ownership |
| Конкурентная | Нестабильный краш — воспроизводится не всегда, зависит от порядка | В DependencyGraph: проверить что нет W/R в одной фазе |

## 7.4 Тесты как доказательство инварианта

В Dual Frontier тест — это не галочка. Это доказательство что конкретный инвариант соблюдается. xUnit + FluentAssertions.

```csharp
// Тест: guard ловит незадекларированное чтение
[Fact]
public void GetComponent_UndeclaredRead_ThrowsIsolationViolation()
{
    var world = new World();
    var ctx = new SystemExecutionContext(world, "TestSystem",
        reads: Array.Empty<Type>(), writes: Array.Empty<Type>(),
        buses: Array.Empty<string>(), origin: SystemOrigin.Core,
        modId: null, faultSink: new NullModFaultSink());
```

```csharp
    SystemExecutionContext.PushContext(ctx);
    try {
        var id = world.CreateEntity();
        world.AddComponent(id, new TestComponent());
```

```csharp
        Action act = () => ctx.GetComponent<TestComponent>(id);
        act.Should().Throw<IsolationViolationException>();
    }
    finally { SystemExecutionContext.PopContext(); }
}
```

# 8. Учебный маршрут — 14 дней

Каждый день: 20–30 мин теории по одной теме → 30–60 мин работа с реальными файлами проекта → 1 практическая проверка → короткая запись в заметках проекта.

## Дни 1–7: Язык и контракты

| День | Тема | Файл в проекте | Практическая проверка |
|---|---|---|---|
| 1 | class vs struct, readonly, nullable | EntityId.cs, ComponentStore.cs | Объяснить: почему EntityId — readonly struct, а не class. Где nullable защищает контракт. |
| 2 | Ссылка vs копия значения | ComponentStore<T>.Get(), SetComponent() | Написать код: изменить HealthComponent через GetComponent/SetComponent. Убедиться что изменение сохранилось. |
| 3 | Интерфейсы как контракт | IComponent, IEvent, IModApi, IGameServices | Ответить: что произойдёт если добавить метод в IComponent. Почему IGameServices — не класс. |
| 4 | Generics: чтение сигнатур | ComponentStore<T>, Query<T1,T2>(), Subscribe<TEvent>() | Прочитать сигнатуру ComponentStore<T> вслух. Объяснить where T : IComponent. |
| 5 | Атрибуты и Reflection | [SystemAccess], DependencyGraph.AddSystem() | Найти в коде где GetCustomAttribute читает атрибут. Добавить тестовую систему без атрибута — проверить краш. |
| 6 | Исключения и finally | ParallelSystemScheduler, SystemExecutionContext | Убрать finally у PopContext в тестовом коде. Запустить тест. Понять что сломалось. |
| 7 | Коллекции в проекте | World._stores, _contextCache, _allowedReads | Для каждого поля назвать: почему именно эта коллекция (порядок? lookup? потокобезопасность?). |

## Дни 8–14: Многопоточность и дебаг

| День | Тема | Файл в проекте | Практическая проверка |
|---|---|---|---|
| 8 | Race conditions: W/W и W/R | DependencyGraph.cs, DependencyGraphTests.cs | Найти тест Build_WriteWriteConflict_Throws. Запустить. Объяснить почему он существует. |
| 9 | ThreadLocal — механика | SystemExecutionContext._current, PushContext/PopContext | Объяснить: почему _current — ThreadLocal, а не static поле. Что произошло бы без ThreadLocal. |
| 10 | Parallel.ForEach и барьер | ParallelSystemScheduler.ExecutePhase(), ExecuteTick() | Нарисовать схему: фазы 0→1→2 и где находится барьер. Почему порядок систем внутри фазы не гарантирован. |
| 11 | TickScheduler — частоты | TickScheduler.cs, TickRates.cs | Ответить: система с [TickRate(SLOW)] сколько раз выполнится за 10 секунд? А REALTIME? |
| 12 | Чтение stack trace | IsolationViolationException в тестах | Запустить isolation-тест на умышленное нарушение. Найти в stack trace строку первого throw. |
| 13 | Тесты как доказательство | DependencyGraphTests.cs, ComponentStoreTests.cs | Прочитать тест Build_LinearChain_OrderedPhases. Объяснить что именно он доказывает об архитектуре. |
| 14 | Ревизия и документация | Все README модулей, ARCHITECTURE.md | Обновить свои заметки. Отметить что стало каноном. Написать один новый тест на понравившийся инвариант. |

> **✅ Ежедневное правило** — Если прочитал код и не можешь объяснить почему он написан именно так — это сигнал для изучения. Не "что делает код", а "какой инвариант он защищает".

# 9. Быстрая шпаргалка — правила проекта

## Архитектурные запреты

| Запрет | Почему | Альтернатива |
|---|---|---|
| async/await в Domain | Продолжение — другой поток, ThreadLocal null, детерминизм сломан | Синхронный код, шины для коммуникации |
| Прямой доступ к World из системы | Обходит guard изоляции, не декларированный доступ | GetComponent<T> через SystemBase (через контекст) |
| GetSystem<TSystem>() в системе | Прямая связь систем — нарушение изоляции | Publish событие в шину |
| Ссылка мода на Core | AssemblyLoadContext блокирует физически | Только DualFrontier.Contracts |
| W/W на одном компоненте | Гонка данных — тихий баг в save | DependencyGraph запрещает при Build() |
| Rebuild() планировщика во время тика | Нарушает _phases под Parallel.ForEach | Только из меню между сессиями |

## Ключевые гарантии ECS

| Гарантия | Где обеспечена |
|---|---|
| В одной фазе нет W/W и W/R на одном компоненте | DependencyGraph.Build() + топологическая сортировка |
| Система видит только задекларированные компоненты (DEBUG) | SystemExecutionContext._allowedReads/_allowedWrites |
| Каждый поток имеет свой независимый контекст | ThreadLocal<SystemExecutionContext?> _current |
| PopContext всегда вызывается, даже при исключении | try/finally в ParallelSystemScheduler.ExecutePhase() |
| Мод не может уронить игру нарушением изоляции | IModFaultSink → ModFaultHandler выгружает мод |
| Следующая фаза стартует только после завершения текущей | Parallel.ForEach блокирует до завершения всех систем фазы |

## Быстрый выбор

| Ситуация | Решение |
|---|---|
| Нужен маленький неизменяемый идентификатор | readonly record struct (EntityId, GridVector) |
| Нужен объект состояния или сервис | class, sealed |
| Нужна типобезопасная коллекция компонентов | ComponentStore<T> where T : IComponent |
| Нужна связь между системами | Publish событие в шину, не GetSystem<T>() |
| Нужен ресурс (патрон, мана, слот инвентаря) | AmmoIntent / ManaIntent → двухшаговая модель |
| Нужно что-то сделать после удаления entity | [Deferred] на DeathEvent |
| Нужно прервать фазу немедленно | [Immediate] — использовать крайне редко |
| Нужен доступ к компоненту из теста без системы | Явный PushContext в тесте, явный PopContext в finally |

Контракты дают форму. Системы дают поведение. Планировщик защищает правила. Тесты доказывают что правила соблюдаются.

Dual Frontier · C# и многопоточность · Учебный материал проекта
