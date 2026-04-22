# Entity Component System

Dual Frontier использует классический подход ECS: entity — это идентификатор, компоненты — чистые данные, системы — логика. Ядро ECS живёт в сборке `DualFrontier.Core` и помечено `internal` — снаружи видны только контракты `IComponent`, `EntityId` и базовый класс `SystemBase`.

## World, EntityId, Component

### World

`World` — реестр всех entities. Для каждого типа компонента хранится свой `ComponentStore<T>`. Мир единственный на игру, создаётся в `GameLoop` и передаётся `ParallelSystemScheduler`. Системы не трогают `World` напрямую — доступ идёт через `SystemExecutionContext`.

```csharp
internal sealed class World
{
    private readonly ConcurrentDictionary<Type, IComponentStore> _stores = new();
    private int _nextEntityId;

    public EntityId CreateEntity() { /* ... */ }
    public void DestroyEntity(EntityId id) { /* ... */ }

    // Небезопасный доступ — только для SystemExecutionContext.
    internal T GetComponentUnsafe<T>(EntityId id) where T : IComponent { /* ... */ }
}
```

### EntityId

`EntityId` — `readonly struct` с двумя полями: `int Index` и `int Version`. Версия инкрементируется при удалении entity, что делает старые ссылки безопасно невалидными. Сравнение `id.Version == world.GetVersion(id.Index)` возвращает `false` для мёртвой ссылки — система спокойно пропускает её, а не крашится.

### Component

Компонент — класс (или `struct`), реализующий `IComponent`. Никакой логики, только данные. Валидация, арифметика, проверки — всё в системах. Это даёт два выигрыша: компоненты свободно сериализуются как POCO, и их можно безопасно читать из нескольких систем одновременно, пока никто не пишет.

## SparseSet — почему не массив

Наивный массив `T[]` для компонентов тратит память: большинство entity не имеют данного компонента. Плотный массив `List<T>` требует бинарного поиска по `EntityId` — O(log n). SparseSet решает обе проблемы.

SparseSet хранит две структуры:

- `sparse[index]` — массив длиной `maxEntityCount`, хранит позицию компонента в dense-массиве или `-1`.
- `dense[]` — плотный массив компонентов и параллельный массив `EntityId`-ов.

Вставка — O(1), удаление — O(1) через swap-with-last, итерация — O(N) по плотному массиву без пропусков. Этот паттерн принят всеми современными ECS-движками (EnTT, bevy_ecs, flecs).

Для Dual Frontier важна именно итерация: системы в `Update` пробегают все entity с нужным набором компонентов. Плотный массив даёт хороший cache locality — существенный выигрыш на 10–20 тысячах entity.

## Query<T1, T2, ...>

Запрос `Query<HealthComponent, PositionComponent>()` возвращает итератор по entity, у которых есть оба компонента. Алгоритм — пересечение sparse-множеств: берётся самый маленький `ComponentStore` и проверяется наличие у каждой entity остальных компонентов.

```csharp
[SystemAccess(reads: new[] { typeof(HealthComponent), typeof(PositionComponent) })]
public class HealthReporterSystem : SystemBase
{
    public override void Update(float delta)
    {
        foreach (var entity in Query<HealthComponent, PositionComponent>())
        {
            var health = GetComponent<HealthComponent>(entity);
            var pos = GetComponent<PositionComponent>(entity);
            // Доступ разрешён — оба компонента задекларированы в reads.
        }
    }
}
```

Query не материализует список — он генерирует entity лениво. Это важно, потому что в `Update` список может измениться (другая система параллельно удалит entity), а генератор просто пропустит устаревшую версию.

## Жизненный цикл entity

### Создание

`world.CreateEntity()` возвращает новый `EntityId` с индексом и версией. Компоненты прикрепляются отдельно: `world.AddComponent(id, new HealthComponent { Maximum = 100 })`. Создавать entity может только `Application` или система через специальный метод `SystemBase.CreateEntity` (регистрируется в `SystemExecutionContext` как write-all).

### Удаление

`world.DestroyEntity(id)` не освобождает индекс сразу — удаление компонентов происходит в конце текущей фазы планировщика, чтобы параллельные системы завершили обход без NullReferenceException. Версия entity инкрементируется. Все последующие `GetComponent<T>(oldId)` возвращают `null` или кидают `IsolationViolationException` при попытке записать.

### Версионирование

Entity со старой версией — индикатор мёртвой ссылки. Системы проверяют валидность через `world.IsAlive(id)`. Никаких `null`-паник: неизменяемые записи события носят `EntityId`, и если к моменту обработки entity уже удалена, обработчик просто выходит. Это делает шину событий устойчивой к гонкам.

## SystemBase

`SystemBase` — базовый класс всех игровых систем. Определяет три точки расширения:

```csharp
public abstract class SystemBase
{
    // Вызывается один раз при регистрации системы.
    // Используется для подписок на шину или другого one-time setup.
    protected virtual void OnInitialize() { }

    // Вызывается планировщиком с частотой, заданной [TickRate].
    public abstract void Update(float delta);

    // Вызывается при выгрузке системы. Отписка от шин, освобождение ресурсов.
    protected virtual void OnDispose() { }

    // Безопасное чтение/запись через сторож.
    protected T GetComponent<T>(EntityId id) where T : IComponent;
    protected void SetComponent<T>(EntityId id, T value) where T : IComponent;
    protected IEnumerable<EntityId> Query<T>() where T : IComponent;
    protected IEnumerable<EntityId> Query<T1, T2>()
        where T1 : IComponent where T2 : IComponent;
    protected TSystem GetSystem<TSystem>() where TSystem : SystemBase; // всегда краш
}
```

Декларация доступа обязательна: `[SystemAccess(reads: [...], writes: [...], bus: nameof(IGameServices.Combat))]`. Планировщик читает атрибут один раз при старте и строит граф зависимостей.

## Anti-patterns

### Хранить ссылку на World в системе

```csharp
// ПЛОХО — обходит сторож, ломает параллелизм.
public class BadSystem : SystemBase
{
    private World _world;
    public BadSystem(World w) { _world = w; }
}
```

Системы получают `World` только через `SystemExecutionContext` — и не хранят его. Пересборка графа или hot-reload мода сломает кешированную ссылку.

### Прямой вызов другой системы

```csharp
// ПЛОХО — обходит шину, создаёт неявную зависимость.
var inventory = GetSystem<InventorySystem>();
inventory.ReserveAmmo(...);
```

Сторож всегда (даже в Release) кидает `IsolationViolationException` на `GetSystem`. Вместо этого — `bus.Inventory.Publish(new AmmoIntent { ... })`.

### Логика в компоненте

```csharp
// ПЛОХО — компонент не чистые данные.
public sealed class HealthComponent : IComponent
{
    public float Current;
    public void TakeDamage(float amount) { Current -= amount; }  // нет.
}
```

Логика урона живёт в `DamageSystem`. Компонент остаётся POCO.

### Создание entity в `Update` с немедленным `GetComponent`

```csharp
// ПЛОХО — другая система в этой же фазе не увидит новой entity.
var id = CreateEntity();
var health = GetComponent<HealthComponent>(id);  // гонка.
```

Новые entity становятся видимы в следующей фазе. Логику дробят: создание в этой фазе, работа с компонентами — в следующей.

## См. также

- [ARCHITECTURE](./ARCHITECTURE.md)
- [ISOLATION](./ISOLATION.md)
- [THREADING](./THREADING.md)
