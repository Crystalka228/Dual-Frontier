---
# Auto-generated from docs/governance/REGISTER.yaml — DO NOT EDIT MANUALLY
# Manual edits overwritten by sync_register.ps1 on next sync.
register_id: DOC-B-TESTING_STRATEGY
category: B
tier: 1
lifecycle: LOCKED
owner: Crystalka
version: "1.0"
next_review_due: 2027-05-12
register_view_url: docs/governance/REGISTER_RENDER.md#DOC-B-TESTING_STRATEGY
---
# Testing strategy

Tests in Dual Frontier are the first line of defense for architectural guarantees. The isolation guard, the dependency graph, the event bus — all of these are useless without tests that fail on regression. The project uses xUnit + FluentAssertions; mocks appear only where a real component cannot be substituted. Test projects are grouped by the assemblies they verify.

## Unit tests

Scope: one class, or one pair of classes, in isolation. Goal: catch logic errors on the minimum surface.

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

Coverage: Add, Remove, Get, Has, Count, iteration, SparseSet growth, version mismatch.

### EventBus

- `Publish` delivers the event to every subscriber.
- `Unsubscribe` removes the handler; the next `Publish` does not call it.
- `Subscribe` during `Publish` does not break iteration.
- A `[Deferred]` event is not delivered synchronously — only after `FlushDeferred`.
- An `[Immediate]` event preempts processing and is delivered ahead of others.

### DependencyGraph

- Two systems without shared WRITES land in the same phase.
- A writer and a reader of the same component land in different phases.
- A cycle in the graph is detected and throws `CyclicDependencyException`.
- A write conflict between two systems in one phase is an error.

## Bus-driven integration tests

Scope: several systems work together through the bus, without mocking the bus. Goal: confirm that the access declaration and the phase order genuinely produce the expected scenario.

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

    fixture.Tick(3); // 3 phases: Intent → Granted → ShootAttempt
    fixture.PublishedEvents<AmmoGranted>().Should().ContainSingle(e => e.RequesterId == shooter);
}
```

`SchedulerFixture` builds a mini scheduler from the real `ParallelSystemScheduler` and the real `World`. No mocks. This is heavier than a unit test, but it covers real interaction.

## Isolation tests — the guard catches

A critically important test class: it confirms that `SystemExecutionContext` actually crashes violators.

```csharp
[Fact]
public void System_reading_undeclared_component_throws_IsolationViolation()
{
    var fixture = new SchedulerFixture().AddSystem<WrongSystem>().Build();
    var action = () => fixture.Tick(1);

    action.Should()
          .Throw<IsolationViolationException>()
          .WithMessage("*WrongSystem*HealthComponent*")
          .Which.Message.Should().Contain("Add: [SystemAccess");
}

[Fact]
public void System_requesting_GetSystem_throws_in_Release_too()
{
    // Even with DEBUG_SYMBOLS_OFF the test must fail.
}

[Fact]
public void System_publishing_to_wrong_bus_throws()
{
    // ...
}
```

Every violation listed in [ISOLATION](/docs/architecture/ISOLATION.md) MUST have a paired test. Without these tests the architectural guarantee remains marketing.

## Modding tests — the mod sees no internals

These verify that `AssemblyLoadContext` physically blocks access to the core.

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

Fixture mods live in `tests/fixtures/*` and are built in CI before the main test run.

## Performance tests

Benchmarks (BenchmarkDotNet) live in a separate `tests/DualFrontier.Core.Benchmarks` project. Zero overhead on a regular test run: benchmarks are not invoked through `dotnet test`, only through `dotnet run -c Release`.

Regression gates in CI: `PerformanceGates.cs` compares results to the baseline and fails when degradation exceeds 10%. Details: [PERFORMANCE](/docs/architecture/PERFORMANCE.md).

## dotnet test

Layout:

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

Local run:

```
dotnet test                          # all tests
dotnet test tests/DualFrontier.Core.Tests
dotnet test --filter "FullyQualifiedName~Isolation"
```

CI gate: red build on any test failure or any performance-threshold violation. Skipping tests with `[Skip]` is not allowed without a documented issue.

## See also

- [ISOLATION](/docs/architecture/ISOLATION.md)
- [PERFORMANCE](/docs/architecture/PERFORMANCE.md)
- [CODING_STANDARDS](./CODING_STANDARDS.md)
