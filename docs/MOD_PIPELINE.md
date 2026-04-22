# Dual Frontier — MOD_PIPELINE

**Модульный конвейер интеграции модов**
Версия архитектуры: v0.2  |  Фаза реализации: 2

## Концепция

Моды регистрируют контракты (компоненты, системы, события) через `IModApi`. Граф
зависимостей (`DependencyGraph`) пересобирается один раз при применении модов —
до начала игровой сессии. В runtime граф и планировщик неизменны.

```
МЕНЮ МОДОВ                       ИГРОВОЙ RUNTIME
─────────────────────────        ────────────────────────────
ModIntegrationPipeline           ParallelSystemScheduler
├── ContractValidator            (неизменен в сессии)
├── ModRegistry
├── DependencyGraph.Reset()
├── DependencyGraph.Build()
└── Scheduler.Rebuild()
```

**Ключевое свойство**: граф всегда консистентен — либо старый рабочий, либо
новый полностью собранный. Частично собранного графа не существует.

## Компоненты

### 1. ModIntegrationPipeline

Оркестратор всего процесса. Вызывается из UI меню при нажатии «Применить моды».

Ответственность: последовательно вызвать остальные компоненты, откатить
изменения при ошибке, вернуть итоговый отчёт.

```
ModIntegrationPipeline.Apply(IReadOnlyList<string> modPaths)
│
├── 1. ModLoader.LoadMod(path)                   → List<LoadedMod>
├── 2. ContractValidator.Validate(mods, core)    → ValidationReport
│       ├── проверка версий ContractsVersion
│       └── pre-registration write-write check
├── 3. IMod.Initialize(RestrictedModApi)         — регистрация в ModRegistry
├── 4. DependencyGraph.Reset() (локальный)
├── 5. DependencyGraph.AddSystems(core + mods)
├── 6. DependencyGraph.Build()                   → List<SystemPhase>
└── 7. ParallelSystemScheduler.Rebuild(phases)
```

### 2. ContractValidator

Двухфазная валидация до регистрации в графе.

**Фаза A — версионирование**: каждый манифест декларирует
`requiresContractsVersion`. Валидатор сравнивает с `ContractsVersion.Current`
и отклоняет моды с `Major != Current.Major` или `Minor/Patch > Current`.

**Фаза B — конфликты компонентов**: до вызова `DependencyGraph.Build()` валидатор
проверяет write-write коллизии между всеми декларируемыми системами (Core +
моды). Даёт точное сообщение: «мод X конфликтует с модом Y по компоненту Z»,
а не просто «write conflict detected» уже внутри графа.

```csharp
internal sealed class ContractValidator
{
    public ValidationReport Validate(
        IReadOnlyList<LoadedMod> mods,
        IReadOnlyList<SystemBase> coreSystems);
}

public sealed record ValidationError(
    string ModId,
    ValidationErrorKind Kind,
    string Message,
    string? ConflictingModId = null,
    Type? ConflictingComponent = null);

public enum ValidationErrorKind
{
    IncompatibleContractsVersion, // мод требует > текущей версии контрактов
    WriteWriteConflict,           // два мода пишут один компонент
    CyclicDependency,             // граф модов содержит цикл
    MissingDependency,            // мод требует другой мод, которого нет
}
```

### 3. ModRegistry

Реестр всех зарегистрированных компонентов и систем от модов. Является backing
storage для `RestrictedModApi`.

```csharp
internal sealed class ModRegistry
{
    public void SetCoreSystems(IReadOnlyList<SystemBase> coreSystems);
    public void RegisterComponent(string modId, Type componentType);
    public void RegisterSystem(string modId, Type systemType);
    public IReadOnlyList<SystemRegistration> GetAllSystems();
    public void ResetModSystems();
    public void RemoveMod(string modId);
}

internal sealed record SystemRegistration(
    SystemBase Instance,
    SystemOrigin Origin,
    string? ModId);
```

Правила:

- `RegisterComponent` бросает `InvalidOperationException`, если тип уже зарегистрирован другим модом (указывает обоих владельцев).
- `RegisterSystem` проверяет наличие `[SystemAccess]` и `[TickRate]`, иначе `InvalidOperationException` с подсказкой.
- `GetAllSystems` возвращает Core-системы первыми, затем мод-системы в порядке регистрации.
- `ResetModSystems` сбрасывает только мод-системы, Core остаётся.

### 4. RestrictedModApi

Реализация `IModApi`, которую `ModLoader` передаёт каждому моду в
`IMod.Initialize`. Проксирует вызовы в ядро через `ModRegistry` и
`IModContractStore`.

```csharp
internal sealed class RestrictedModApi : IModApi
{
    internal RestrictedModApi(
        string modId,
        ModRegistry registry,
        IModContractStore contractStore,
        IGameServices services);

    public void RegisterComponent<T>() where T : IComponent;
    public void RegisterSystem<T>() where T : class;
    public void PublishContract<T>(T c) where T : IModContract;
    public bool TryGetContract<T>(out T? c) where T : class, IModContract;
    public void Subscribe<T>(Action<T> handler) where T : IEvent;
    public void Publish<T>(T evt) where T : IEvent;

    internal void UnsubscribeAll();
}
```

Подписки (`Subscribe<T>`) трекаются в приватном
`List<(Type eventType, Delegate handler)> _subscriptions` для снятия при
`Unload`. Маршрутизация событий по конкретным шинам — в следующей подфазе.

### 5. IModContractStore

Реестр межмодовых контрактов (`IModContract`). Отдельный от `ModRegistry`
компонент, так как контракты живут в runtime (а не только при загрузке).

```csharp
internal interface IModContractStore
{
    void Publish<T>(string modId, T contract) where T : IModContract;
    bool TryGet<T>(out T? contract) where T : class, IModContract;
    void RevokeAll(string modId);
}
```

При выгрузке мода-поставщика: `RevokeAll(modId)` отзывает все его контракты.
Последующие `TryGetContract` вернут `false`.

## Жизненный цикл

### Загрузка модов (в меню)

```
Пользователь нажимает «Применить»
      │
      ▼
ModIntegrationPipeline.Apply()
      │
      ├── [1] ModLoader: читает mod.manifest.json
      │         создаёт ModLoadContext (AssemblyLoadContext, isCollectible: true)
      │         находит IMod через reflection
      │
      ├── [2] ContractValidator.Validate()
      │         при ошибках Pipeline останавливается,
      │         возвращает ValidationReport в UI
      │
      ├── [3] IMod.Initialize(RestrictedModApi)
      │         мод регистрирует компоненты/системы
      │         мод подписывается на события
      │
      ├── [4] DependencyGraph.Reset() + AddSystems() + Build()
      │         граф пересобирается с Core + мод-системами
      │
      └── [5] ParallelSystemScheduler.Rebuild(phases)
                новый планировщик готов к следующей сессии
```

### Выгрузка мода

```
ModLoader.UnloadMod(modId)
      │
      ├── [1] ModRegistry.ResetModSystems()       — удалить системы мода
      ├── [2] IModContractStore.RevokeAll(modId)  — снять межмодовые контракты
      ├── [3] EventBus: снять все подписки мода
      ├── [4] IMod.Unload() с таймаутом 500ms
      ├── [5] ModLoadContext.Unload()             — выгрузить AssemblyLoadContext
      ├── [6] DependencyGraph.Reset() + Build()   — пересобрать без мода
      └── [7] Publish ModDisabledEvent            — UI показывает баннер
```

### ModFaultHandler (runtime нарушение)

Если мод-система нарушает изоляцию в runtime (не в меню), `ModFaultHandler`
выполняет ту же цепочку, но шаг **[6] пропускается** (нельзя пересобирать граф
в runtime). Вместо этого: системы мода помечаются как `Disabled` в планировщике,
граф пересобирается при следующем открытии меню модов.

## Структура файлов

```
src/DualFrontier.Application/Modding/
├── ModIntegrationPipeline.cs ← оркестратор
├── ContractValidator.cs      ← валидация
├── ModRegistry.cs            ← реестр систем/компонентов
├── IModContractStore.cs      ← интерфейс межмодовых контрактов
├── ModContractStore.cs       ← реализация
├── ModLoader.cs              ← загрузка .manifest.json + AssemblyLoadContext
├── ModLoadContext.cs         ← свой AssemblyLoadContext на каждый мод
├── RestrictedModApi.cs       ← реализация IModApi
├── ValidationError.cs        ← ValidationError / ValidationWarning / Kind
├── ValidationReport.cs       ← ValidationReport + Ok()
├── LoadedMod.cs              ← результат загрузки одного мода
├── SystemRegistration.cs     ← запись в реестре систем
└── ModIsolationException.cs

src/DualFrontier.Contracts/Modding/
├── IModApi.cs                ← стабилен
├── IMod.cs                   ← стабилен
├── IModContract.cs           ← стабилен
├── ModManifest.cs            ← + RequiresContractsVersion
└── ContractsVersion.cs       ← Parse/IsCompatible/Current

tests/DualFrontier.Modding.Tests/
├── Pipeline/
│   ├── ModIntegrationPipelineTests.cs
│   └── ContractValidatorTests.cs
└── Fixtures/
    └── GoodMod/
        └── GoodMod.cs        ← легальный тестовый мод
```

## API компонентов

### ModIntegrationPipeline

```csharp
internal sealed class ModIntegrationPipeline
{
    /// <summary>
    /// Применяет список модов: загружает, валидирует, регистрирует,
    /// пересобирает граф и планировщик. Вызывается только из меню.
    /// </summary>
    public PipelineResult Apply(IReadOnlyList<string> modPaths);

    /// <summary>
    /// Выгружает все активные моды и возвращает планировщик
    /// к состоянию только с Core-системами.
    /// </summary>
    public void UnloadAll();
}

public sealed record PipelineResult(
    bool Success,
    IReadOnlyList<ValidationError> Errors,
    IReadOnlyList<string> LoadedModIds,
    IReadOnlyList<string> FailedModIds);
```

### ContractsVersion

```csharp
public readonly struct ContractsVersion
{
    public static readonly ContractsVersion Current = new(1, 0, 0);
    public static ContractsVersion Parse(string text);
    public static bool IsCompatible(ContractsVersion required, ContractsVersion available);
}
```

## Обработка ошибок

Стратегия: fail-fast в меню, graceful в runtime.

| Где                 | Ошибка                          | Поведение                                        |
|---------------------|---------------------------------|--------------------------------------------------|
| Меню — валидация    | `IncompatibleContractsVersion`  | Мод не загружается, показать версию              |
| Меню — валидация    | `WriteWriteConflict`            | Оба мода не загружаются, указать компонент       |
| Меню — валидация    | `MissingDependency`             | Зависящий мод не загружается                     |
| Меню — `Build()`    | Цикл в графе                    | Полная отмена, старый планировщик сохраняется    |
| Runtime             | `ModIsolationException`         | `ModFaultHandler`: мод выгружается, игра живёт   |
| Runtime             | Любое исключение из мод-сборки  | Тот же `ModFaultHandler`                         |

## Атомарность пересборки графа

`DependencyGraph.Reset()` + `Build()` выполняются в **локальной переменной**.
Планировщик заменяется только после успешного `Build()`. При ошибке старый
планировщик остаётся активным.

```csharp
// Псевдокод внутри Pipeline
var newGraph = new DependencyGraph();
foreach (var system in _registry.GetAllSystems())
    newGraph.AddSystem(system.Instance);
newGraph.Build();                     // если бросит — планировщик не тронут

// только здесь заменяем фазы существующего планировщика
_scheduler.Rebuild(newGraph.GetPhases());
```

## Тесты

### Pipeline тесты

- `Pipeline_with_valid_mod_loads_and_rebuilds_scheduler`
- `Pipeline_with_version_conflict_rejects_mod_keeps_old_scheduler`
- `Pipeline_with_write_conflict_rejects_both_mods_with_precise_message`
- `Pipeline_build_failure_leaves_old_scheduler_intact`
- `Pipeline_unload_removes_mod_systems_from_scheduler`

### Validator тесты

- `Validator_rejects_mod_requiring_newer_contracts_version`
- `Validator_detects_write_conflict_between_two_mods`
- `Validator_detects_write_conflict_between_mod_and_core`
- `Validator_valid_mods_return_empty_errors`
- `Validator_reports_precise_component_in_conflict_message`
- `Validator_ok_for_compatible_older_patch_version`

## Критерии приёмки (Фаза 2)

- `ModLoader.LoadMod` читает манифест, создаёт `AssemblyLoadContext`, вызывает `IMod.Initialize`.
- `ContractValidator` возвращает точное сообщение при write-write конфликте между модами.
- `ModIntegrationPipeline.Apply` атомарна: ошибка на любом шаге не ломает текущий планировщик.
- Тест: мод с `RequiresContractsVersion: "2.0.0"` при текущей версии 1.x не загружается.
- Тест: два мода, пишущих один компонент — оба отклонены с указанием компонента-конфликта.
- Тест: `ModLoadContext.Unload` физически освобождает сборку (WeakReference тест — часть 2).
- Все тесты из раздела «Тесты» зелёные.

## См. также

- [MODDING](./MODDING.md) — руководство для авторов модов
- [ISOLATION](./ISOLATION.md) — сторож изоляции и ModFaultHandler
- [CONTRACTS](./CONTRACTS.md) — версионирование контрактов
- [THREADING](./THREADING.md) — DependencyGraph и планировщик
- [ROADMAP](./ROADMAP.md) — Фаза 2 (критерии приёмки модинга)
