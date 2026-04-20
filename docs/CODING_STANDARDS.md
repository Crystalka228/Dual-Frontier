# Стандарты кода

Единый стиль кода — это не эстетика, а инструмент навигации: читая чужой файл, разработчик не тратит внимание на расшифровку чужих привычек. В Dual Frontier стандарты формализованы и проверяются анализатором; отклонения ловятся на `dotnet build` с `TreatWarningsAsErrors`.

## Naming

### Публичные и protected члены — PascalCase

```csharp
public sealed class HealthComponent : IComponent
{
    public float Current;
    public float Maximum;
    public void TakeDamage(float amount) { /* ... */ }
}
```

### Приватные поля — _camelCase

```csharp
private readonly Dictionary<Type, IComponentStore> _stores = new();
private int _nextEntityId;
```

Подчёркивание отличает приватное поле от параметра метода и локальной переменной в один взгляд. IDE подсвечивает иначе, но код всё равно должен читаться в `git diff` и `github review`.

### Константы — PascalCase

```csharp
public const int MaxPawnsPerColony = 100;
```

### Интерфейсы — префикс `I`

```csharp
public interface IEventBus { /* ... */ }
public interface IComponent { }
```

### Generic-параметры — `T` или `TContext`-стиль

```csharp
public interface IComponentStore<T> where T : IComponent { /* ... */ }
```

### Файлы и namespace совпадают с путём

Файл `src/DualFrontier.Core/ECS/World.cs` содержит `namespace DualFrontier.Core.ECS;` и один тип `World`. Обратное тоже верно: namespace всегда отражает физическую структуру каталогов.

## File-scoped namespaces

Используется только file-scoped синтаксис (C# 10+):

```csharp
// ПРАВИЛЬНО
namespace DualFrontier.Core.ECS;

public sealed class World { /* ... */ }

// НЕПРАВИЛЬНО — лишний уровень вложенности
namespace DualFrontier.Core.ECS
{
    public sealed class World { /* ... */ }
}
```

Меньше отступов → больше полезного кода на экран. Проверяется анализатором IDE0161.

## Nullable enabled

Все проекты собираются с `<Nullable>enable</Nullable>` в `Directory.Build.props`. Никаких `#nullable disable`, никаких `!` без обоснования.

```csharp
// ПРАВИЛЬНО
public string? OptionalName { get; init; }
public string RequiredName { get; init; } = "";

// ПЛОХО
public string SomeName { get; init; } = null!;  // только в DTO с required и init-only.
```

Если API должен возвращать `null`, он возвращает `T?`. Если не должен — возвращает `T`. `null` в непометенном поле — повод для баг-репорта.

## Комментарии на русском — доменная логика

Внутренняя доменная логика прокомментирована на русском. Это язык проекта: GDD, тех-архитектура, issues, ревью. Мешать русский и английский в одном файле комментарии не нужно.

```csharp
// Эфирный срыв: маг работает с кристаллом выше своего уровня.
// По формуле из GDD 4.2: шанс провала = (кристаллТир - магУровень) × 0.25.
if (crystal.Tier > mage.EtherLevel)
{
    var failChance = (crystal.Tier - mage.EtherLevel) * 0.25f;
    // ...
}
```

Доменные термины фиксируются на русском: "пешка", "голем", "эфирный узел", "ритуал", "кристалл-боеприпас".

## XML docs на английском — публичный API

Публичный API — всё, что видно за пределами сборки. Для него XML-документация пишется по-английски: документация генерируется в `bin/` и поставляется вместе с nuget-пакетом, её может читать любой сторонний разработчик, в том числе не говорящий по-русски.

```csharp
/// <summary>
/// Publishes an event to the domain bus. The event is delivered synchronously
/// to all subscribers within the current scheduler phase. Use [Deferred] for
/// cross-phase delivery, [Immediate] to interrupt the phase.
/// </summary>
/// <typeparam name="T">Event type; must implement IEvent.</typeparam>
/// <param name="evt">The event instance to publish.</param>
public void Publish<T>(T evt) where T : IEvent;
```

Правило разделения: `/// <summary>` + `<remarks>` на английском; `//` inline-комментарий про конкретную формулу или геймдизайн — на русском.

## Один класс — один файл

Принцип навигации: открыл IDE → ввёл имя класса → увидел файл. Исключения очень узкие:

- Enum или `readonly struct` длиной до 10 строк, логически парные основному классу (например, `HealthComponent` и связанный с ним `HealthStatus` enum).
- Private вложенный класс, логически неотрывный от owner (state machine sub-classes).

Никаких файлов с 5+ классами. Никаких "утилитных" свалок `Helpers.cs` на 500 строк.

## Порядок членов класса

Единый порядок снизу вверх: чем ближе к верху — тем раньше нужно при чтении.

1. `const`-поля.
2. `static readonly` поля.
3. `private readonly` поля.
4. `private` мутабельные поля.
5. Конструкторы.
6. `public` свойства.
7. `public` методы.
8. `protected` методы.
9. `private` методы.
10. Вложенные типы.

```csharp
public sealed class DomainEventBus : IEventBus
{
    private const int InitialCapacity = 16;

    private static readonly object SubscribeLock = new();

    private readonly ConcurrentDictionary<Type, Delegate> _subscribers = new();
    private int _publishCount;

    public DomainEventBus() { }

    public int PublishCount => _publishCount;

    public void Publish<T>(T evt) where T : IEvent { /* ... */ }
    public void Subscribe<T>(Action<T> handler) where T : IEvent { /* ... */ }

    private void RecordMetric() { /* ... */ }
}
```

Ревьюер видит сразу — какие поля, какой ctor, какой публичный API. Не надо скроллить.

## Дополнительные правила

- `using` — sorted, сначала `System.*`, потом `DualFrontier.*`, потом прочие. Автоформат IDE.
- `var` — когда тип очевиден из RHS (`new`, cast, factory). Иначе — явный тип.
- `async` запрещён в Domain (см. [THREADING](./THREADING.md)). Разрешён в Application и Presentation.
- Магические числа — `const` с именем, описывающим смысл. `4.2f` в коде без комментария — смертный грех.
- Возврат `null` из публичного API — только когда это явная часть контракта (`TryGet` и `T? FindBy(...)`).

## См. также

- [ARCHITECTURE](./ARCHITECTURE.md)
- [TESTING_STRATEGY](./TESTING_STRATEGY.md)
- [ISOLATION](./ISOLATION.md)
