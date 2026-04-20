# Стратегия тестирования

Тесты в Dual Frontier — первая линия защиты архитектурных гарантий. Сторож изоляции, граф зависимостей, шина событий — всё это бесполезно, если нет тестов, которые при регрессии падают. В проекте используется xUnit + FluentAssertions; моки — только там, где нельзя подставить реальный компонент. Тестовые проекты сгруппированы по сборкам, которые проверяют.

## Unit тесты

Уровень: один класс или одна пара классов в изоляции. Цель: поймать логические ошибки на минимальной поверхности.

### ComponentStore

```csharp
public class ComponentStoreTests
{
    [Fact]
    public void Add_then_Get_returns_same_instance()
    {
        var store = new ComponentStore<HealthComponent>();
        var entity = new EntityId(42, 1);
        var health = new HealthComponent { Maximum = 100 };

        store.Add(entity, health);

        store.Get(entity).Should().BeSameAs(health);
    }

    [Fact]
    public void Remove_then_Get_returns_null()
    {
        // ...
    }

    [Fact]
    public void Destroyed_entity_with_old_version_returns_null()
    {
        // ...
    }
}
```

Покрытие: Add, Remove, Get, Has, Count, iteration, SparseSet growth, version mismatch.

### EventBus

- `Publish` доставляет событие всем подписчикам.
- `Unsubscribe` удаляет обработчик, следующий `Publish` его не вызывает.
- `Subscribe` во время `Publish` не ломает итерацию.
- `[Deferred]` событие не доставляется синхронно — только после `FlushDeferred`.
- `[Immediate]` событие прерывает обработку и доставляется раньше остальных.

### DependencyGraph

- Две системы без общих WRITE попадают в одну фазу.
- Система-писатель и система-читатель одного компонента — в разных фазах.
- Цикл в графе обнаруживается и кидает `CyclicDependencyException`.
- Конфликт записи двумя системами в одну фазу — ошибка.

## Integration тесты через шину

Уровень: несколько систем работают вместе через шину, без мока шины. Цель: убедиться, что декларация доступа и порядок фаз действительно производят ожидаемый сценарий.

```csharp
[Fact]
public void CombatSystem_publishes_AmmoIntent_InventorySystem_responds_with_Granted()
{
    using var fixture = new SchedulerFixture()
        .AddSystem<CombatSystem>()
        .AddSystem<InventorySystem>()
        .Build();

    var shooter = fixture.World.CreateEntity();
    fixture.World.AddComponent(shooter, new WeaponComponent { RequiredAmmo = AmmoType.Rifle });
    fixture.World.AddComponent(shooter, new PositionComponent { Position = new GridVector(0, 0) });

    fixture.Tick(3); // 3 фазы: Intent → Granted → ShootAttempt
    fixture.PublishedEvents<AmmoGranted>().Should().ContainSingle(e => e.RequesterId == shooter);
}
```

`SchedulerFixture` собирает мини-планировщик из реального `ParallelSystemScheduler` и реального `World`. Моков нет. Это тяжелее unit-теста, но покрывает реальное взаимодействие.

## Isolation тесты — сторож ловит

Критически важный класс тестов: подтверждают, что `SystemExecutionContext` действительно крашит нарушителей.

```csharp
[Fact]
public void System_reading_undeclared_component_throws_IsolationViolation()
{
    var fixture = new SchedulerFixture().AddSystem<WrongSystem>().Build();
    var action = () => fixture.Tick(1);

    action.Should()
          .Throw<IsolationViolationException>()
          .WithMessage("*WrongSystem*HealthComponent*")
          .Which.Message.Should().Contain("Добавь: [SystemAccess");
}

[Fact]
public void System_requesting_GetSystem_throws_in_Release_too()
{
    // Даже с DEBUG_SYMBOLS_OFF тест должен падать.
}

[Fact]
public void System_publishing_to_wrong_bus_throws()
{
    // ...
}
```

Список всех нарушений из [ISOLATION](./ISOLATION.md) должен иметь парный тест. Без этих тестов архитектурная гарантия остаётся маркетингом.

## Modding тесты — мод не видит internals

Проверяют, что `AssemblyLoadContext` физически блокирует доступ к ядру.

```csharp
[Fact]
public void Mod_cannot_load_DualFrontierCore_assembly()
{
    using var loader = new ModLoader();
    var action = () => loader.Load("tests/fixtures/EvilMod/EvilMod.dll");

    action.Should()
          .Throw<ModIsolationException>()
          .WithMessage("*DualFrontier.Core*");
}

[Fact]
public void Mod_calling_World_directly_throws_at_compile_time() { /* ... */ }

[Fact]
public void Mod_can_use_IModApi_to_register_system()
{
    using var loader = new ModLoader();
    loader.Load("tests/fixtures/GoodMod/GoodMod.dll");

    loader.RegisteredSystems.Should().Contain(s => s.Name == "MyModSystem");
}

[Fact]
public void Mod_Unload_releases_AssemblyLoadContext()
{
    var weakRef = LoadAndUnloadMod();
    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();
    weakRef.IsAlive.Should().BeFalse();
}
```

Тестовые fixture-моды живут в `tests/fixtures/*` и собираются в CI перед основным test-run.

## Performance тесты

Бенчмарки (BenchmarkDotNet) в отдельном проекте `tests/DualFrontier.Core.Benchmarks`. Нулевой оверхед на обычный test-run: бенчмарки не запускаются через `dotnet test`, только через `dotnet run -c Release`.

Regression gates в CI: `PerformanceGates.cs` сравнивает результаты с baseline и падает при ухудшении больше чем на 10%. Подробности — [PERFORMANCE](./PERFORMANCE.md).

## dotnet test

Структура:

```
tests/
  DualFrontier.Core.Tests/            # unit + isolation
    DualFrontier.Core.Tests.csproj
    ECS/
      ComponentStoreTests.cs
      WorldTests.cs
      QueryTests.cs
    Scheduling/
      DependencyGraphTests.cs
      ParallelSchedulerTests.cs
    Bus/
      DomainEventBusTests.cs
    Isolation/
      IsolationViolationTests.cs

  DualFrontier.Systems.Tests/         # integration
    DualFrontier.Systems.Tests.csproj

  DualFrontier.Modding.Tests/         # modding/assembly isolation
    DualFrontier.Modding.Tests.csproj

  DualFrontier.Core.Benchmarks/       # BenchmarkDotNet
```

Локальный прогон:

```
dotnet test                          # все тесты
dotnet test tests/DualFrontier.Core.Tests
dotnet test --filter "FullyQualifiedName~Isolation"
```

CI gate: красная сборка при падении любого теста или при нарушении performance-порога. Пропускать тесты флагом `[Skip]` нельзя без задокументированной issue.

## См. также

- [ISOLATION](./ISOLATION.md)
- [PERFORMANCE](./PERFORMANCE.md)
- [CODING_STANDARDS](./CODING_STANDARDS.md)
