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

## Сообщения коммитов

Каждый коммит начинается со scope-префикса. Префикс задаёт сторону границы
движок / игра (см. [ARCHITECTURE §«Граница движок / игра»](./ARCHITECTURE.md#граница-движок--игра))
и одной командой `git log --grep` позволяет извлечь историю любой
подсистемы. Это инвестиция в дешевизну форка движка после Phase 7.

### Движковые префиксы

Используются в коммитах, трогающих исключительно движковые сборки.
После релиза эти коммиты уходят в форк движка.

| Префикс | Где применяется |
|---|---|
| `contracts:` | `DualFrontier.Contracts` — публичные интерфейсы, атрибуты, маркер-типы. |
| `core:` | `DualFrontier.Core` — ECS-ядро, планировщик, доменные шины. |
| `interop:` | `DualFrontier.Core.Interop` — P/Invoke-обёртки. |
| `native:` | `native/DualFrontier.Core.Native/` — C++-реализация ядра. |
| `modding:` | Модинг-секция `DualFrontier.Application` — `ModLoader`, `ContractValidator` и пр. |
| `presentation-native:` | `DualFrontier.Presentation.Native` — Silk.NET production-рантайм. |
| `experiment:` | Исследовательские ветки до мёржа. После принятия эксперимент превращается в обычный движковый коммит. |

### Игровые префиксы

Используются в коммитах, трогающих исключительно игровые сборки.
После релиза эти коммиты остаются в основном репозитории игры.

| Префикс | Область |
|---|---|
| `feat(pawn):`, `fix(pawn):` | Пешки — нужды, настроение, jobs. |
| `feat(combat):`, `fix(combat):` | Бой, оружие, броня, статусы. |
| `feat(magic):`, `fix(magic):` | Магия, мана, голем, эфир. |
| `feat(world):`, `fix(world):` | Мир, биомы, погода, рейды. |
| `feat(inventory):`, `fix(inventory):` | Инвентарь, склады, крафт. |
| `feat(ai):`, `fix(ai):` | Behavior Tree, jobs-система. |
| `feat(application):`, `fix(application):` | Игровой цикл, ScenarioLoader, SaveSystem. |
| `feat(presentation):`, `fix(presentation):` | Godot DevKit, UI, ноды. |
| `feat(bootstrap):` | Проводка при появлении новой системы — регистрация в `GameBootstrap`. |

### Нейтральные префиксы

Не привязаны к стороне границы.

- `docs:` — документация.
- `test:` — тесты, не привязанные к одной области.
- `chore:` — обслуживание (tooling, конфиги, форматтинг).
- `build:` — сборка (`*.csproj`, `Directory.Build.props`, `CMakeLists.txt`).
- `refactor:` — переорганизация без изменения поведения.

### Машинно-проверяемое условие

Список разрешённых префиксов фиксирован. Скрипт PR-чек-листа из
[DEVELOPMENT_HYGIENE §«Чек-лист перед каждым PR»](./DEVELOPMENT_HYGIENE.md#чек-лист-перед-каждым-pr)
отвергает любой коммит, не начинающийся ни с одного из них. Изменение
списка — архитектурное решение, требующее одновременной правки этого
документа и PR-чек-листа.

### Тело сообщения

После префикса — короткое описание (≤72 символа) в повелительном
наклонении и нижнем регистре после двоеточия:

```
core: implement [Deferred]/[Immediate] event delivery
feat(combat): add ShieldRefillIntent to combat bus
fix(inventory): replace return with continue in HaulSystem
docs: synchronize doc-set after v0.3 architectural shift
```

Развёрнутое описание (если нужно) — после пустой строки. Один коммит =
одна логически завершённая правка; смешанные коммиты («core: ..., also
feat(pawn): ...») разбиваются на два через `git rebase -i` (см. красный
флаг в [DEVELOPMENT_HYGIENE §«Красные флаги»](./DEVELOPMENT_HYGIENE.md#красные-флаги)).

## См. также

- [ARCHITECTURE](./ARCHITECTURE.md)
- [TESTING_STRATEGY](./TESTING_STRATEGY.md)
- [ISOLATION](./ISOLATION.md)
- [DEVELOPMENT_HYGIENE](./DEVELOPMENT_HYGIENE.md)
