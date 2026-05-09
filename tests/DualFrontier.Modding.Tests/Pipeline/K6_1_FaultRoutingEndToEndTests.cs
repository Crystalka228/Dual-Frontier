using System;
using System.Collections.Generic;
using DualFrontier.Application.Modding;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Bus;
using DualFrontier.Core.ECS;
using DualFrontier.Core.Scheduling;
using DualFrontier.Modding.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace DualFrontier.Modding.Tests.Pipeline;

/// <summary>
/// K6.1 — end-to-end coverage for the mod-fault wiring path the K6
/// closure brief left as deferred scope. Verifies that an isolation
/// violation in a mod-origin system reaches
/// <see cref="ModFaultHandler.ReportFault"/> through the actual
/// <see cref="ParallelSystemScheduler"/> + <see cref="SystemExecutionContext"/>
/// path (not via direct ReportFault calls as in the K6 unit tests),
/// and that a subsequent <see cref="ModIntegrationPipeline.Apply"/>
/// drains the queued fault.
///
/// Test fixtures are kept in-process: <see cref="ModFaultingSystem"/>
/// declares no writes and writes <see cref="K6_1_TestComponent"/>
/// during <c>Update</c>, which trips the DEBUG isolation guard. The
/// scheduler is constructed with a <see cref="SystemMetadata"/> table
/// claiming each test system is mod-origin (or core, where called
/// out), which is the production wiring after K6.1's
/// <c>SystemMetadataBuilder.Build</c> projection.
/// </summary>
public sealed class K6_1_FaultRoutingEndToEndTests
{
    private const string ModA = "tests.k6_1.mod-a";
    private const string ModB = "tests.k6_1.mod-b";

    [Fact]
    public void ModSystem_IsolationViolation_ReportsFaultToHandler()
    {
        // K6.1 — mod-origin system writes an undeclared component; the
        // isolation guard's RouteAndThrow path must call ReportFault on
        // the wired sink BEFORE it throws IsolationViolationException.
        var system = new ModFaultingSystem();
        ModFaultHandler handler = BuildSchedulerWithModSystem(system, ModA, out ParallelSystemScheduler scheduler);

        Action act = () => scheduler.ExecuteTick(0.016f);

        act.Should().Throw<AggregateException>()
            .WithInnerException<IsolationViolationException>();
        handler.GetFaultedMods().Should().ContainSingle().Which.Should().Be(ModA);
    }

    [Fact]
    public void ModSystem_IsolationViolation_NextApplyDrainsHandler()
    {
        // K6.1 — pipeline.Apply([]) drains the handler at step [-1].
        // The faulted id is not in _activeMods (we registered the
        // system through the scheduler metadata, not via Apply), so
        // UnloadMod is a no-op per its idempotency contract; the drain
        // still calls ClearFault, leaving the handler empty.
        var system = new ModFaultingSystem();
        ModFaultHandler handler = BuildPipelineWithModSystem(system, ModA, out ModIntegrationPipeline pipeline, out ParallelSystemScheduler scheduler);

        try { scheduler.ExecuteTick(0.016f); } catch (AggregateException) { /* expected */ }
        handler.GetFaultedMods().Should().ContainSingle();

        PipelineResult result = pipeline.Apply(Array.Empty<string>());

        result.Success.Should().BeTrue("empty mod batch is a valid Apply");
        handler.GetFaultedMods().Should().BeEmpty(
            "Apply step [-1] drains every queued fault");
    }

    [Fact]
    public void CoreSystem_IsolationViolation_ThrowsButHandlerEmpty()
    {
        // K6.1 — core systems still propagate IsolationViolationException
        // (developer bug → crash semantics per MOD_OS §10.3) and do NOT
        // route through the fault sink. The handler stays empty.
        var system = new CoreFaultingSystem();
        ModFaultHandler handler = BuildSchedulerWithCoreSystem(system, out ParallelSystemScheduler scheduler);

        Action act = () => scheduler.ExecuteTick(0.016f);

        act.Should().Throw<AggregateException>()
            .WithInnerException<IsolationViolationException>();
        handler.GetFaultedMods().Should().BeEmpty(
            "core-origin faults do not route through IModFaultSink");
    }

    [Fact]
    public void MultipleModSystems_BothFault_AllIdsRecorded()
    {
        // K6.1 — two mod-origin systems with different modIds both
        // commit isolation violations within the same tick. The
        // handler's set deduplicates by id, so each modId appears once.
        var systemA = new ModFaultingSystem();
        var systemB = new ModFaultingSystem();
        var handler = new ModFaultHandler();
        var world = new World();
        var ticks = new TickScheduler();
        var phases = new[]
        {
            new SystemPhase(new SystemBase[] { systemA, systemB }),
        };
        var metadata = new Dictionary<SystemBase, SystemMetadata>
        {
            [systemA] = new SystemMetadata(SystemOrigin.Mod, ModA),
            [systemB] = new SystemMetadata(SystemOrigin.Mod, ModB),
        };
        var scheduler = new ParallelSystemScheduler(phases, ticks, world, metadata, handler);

        try { scheduler.ExecuteTick(0.016f); } catch (AggregateException) { /* expected */ }

        IReadOnlyList<string> faulted = handler.GetFaultedMods();
        faulted.Should().HaveCount(2);
        faulted.Should().Contain(new[] { ModA, ModB });
    }

    [Fact]
    public void MultipleModSystems_OnlyOneFaults_OtherUnaffected()
    {
        // K6.1 — only the faulting system's modId is recorded; the
        // benign system's id stays out of the handler. Verifies the
        // metadata propagation is per-system, not per-tick (every
        // system gets its own SystemExecutionContext with its own
        // origin/modId).
        var benign = new ModBenignSystem();
        var faulting = new ModFaultingSystem();
        var handler = new ModFaultHandler();
        var world = new World();
        var ticks = new TickScheduler();
        var phases = new[]
        {
            new SystemPhase(new SystemBase[] { benign, faulting }),
        };
        var metadata = new Dictionary<SystemBase, SystemMetadata>
        {
            [benign] = new SystemMetadata(SystemOrigin.Mod, ModA),
            [faulting] = new SystemMetadata(SystemOrigin.Mod, ModB),
        };
        var scheduler = new ParallelSystemScheduler(phases, ticks, world, metadata, handler);

        try { scheduler.ExecuteTick(0.016f); } catch (AggregateException) { /* expected */ }

        handler.GetFaultedMods().Should().ContainSingle().Which.Should().Be(ModB);
    }

    [Fact]
    public void ModSystemFaultDuringInitialize_RecordedBeforeCtorThrows()
    {
        // K6.1 — InitializeAllSystems runs from the scheduler ctor with
        // PushContext active, so an isolation violation thrown by
        // SystemBase.Initialize routes through the same RouteAndThrow
        // path as Update-time violations. The handler captures the
        // fault before the ctor's exception propagates.
        var system = new ModFaultingInInitializeSystem();
        var handler = new ModFaultHandler();
        var world = new World();
        var ticks = new TickScheduler();
        var phases = new[] { new SystemPhase(new SystemBase[] { system }) };
        var metadata = new Dictionary<SystemBase, SystemMetadata>
        {
            [system] = new SystemMetadata(SystemOrigin.Mod, ModA),
        };

        Action act = () => new ParallelSystemScheduler(phases, ticks, world, metadata, handler);

        act.Should().Throw<IsolationViolationException>(
            "Initialize-time violation propagates synchronously from the scheduler ctor");
        handler.GetFaultedMods().Should().ContainSingle().Which.Should().Be(ModA);
    }

    private static ModFaultHandler BuildSchedulerWithModSystem(
        SystemBase system,
        string modId,
        out ParallelSystemScheduler scheduler)
    {
        var handler = new ModFaultHandler();
        var world = new World();
        var ticks = new TickScheduler();
        var phases = new[] { new SystemPhase(new SystemBase[] { system }) };
        var metadata = new Dictionary<SystemBase, SystemMetadata>
        {
            [system] = new SystemMetadata(SystemOrigin.Mod, modId),
        };
        scheduler = new ParallelSystemScheduler(phases, ticks, world, metadata, handler);
        return handler;
    }

    private static ModFaultHandler BuildSchedulerWithCoreSystem(
        SystemBase system,
        out ParallelSystemScheduler scheduler)
    {
        var handler = new ModFaultHandler();
        var world = new World();
        var ticks = new TickScheduler();
        var phases = new[] { new SystemPhase(new SystemBase[] { system }) };
        // Core origin metadata — though absent entries default to Core
        // anyway, the explicit entry documents the intent.
        var metadata = new Dictionary<SystemBase, SystemMetadata>
        {
            [system] = new SystemMetadata(SystemOrigin.Core, ModId: null),
        };
        scheduler = new ParallelSystemScheduler(phases, ticks, world, metadata, handler);
        return handler;
    }

    private static ModFaultHandler BuildPipelineWithModSystem(
        SystemBase system,
        string modId,
        out ModIntegrationPipeline pipeline,
        out ParallelSystemScheduler scheduler)
    {
        var loader = new ModLoader();
        var registry = new ModRegistry();
        registry.SetCoreSystems(Array.Empty<SystemBase>());
        var validator = new ContractValidator();
        var contractStore = new ModContractStore();
        IGameServices services = new GameServices();
        var world = new World();
        var ticks = new TickScheduler();
        var phases = new[] { new SystemPhase(new SystemBase[] { system }) };
        var metadata = new Dictionary<SystemBase, SystemMetadata>
        {
            [system] = new SystemMetadata(SystemOrigin.Mod, modId),
        };
        var handler = new ModFaultHandler();
        loader.SetFaultHandler(handler);
        scheduler = new ParallelSystemScheduler(phases, ticks, world, metadata, handler, services);
        pipeline = new ModIntegrationPipeline(
            loader, registry, validator, contractStore, services, scheduler, handler);
        return handler;
    }
}

/// <summary>
/// K6.1 test fixture component — the target of the isolation
/// violations exercised by <see cref="K6_1_FaultRoutingEndToEndTests"/>.
/// Public so the test systems below can write it; lives in this file
/// rather than <c>Components/</c> to keep the fixture local.
/// </summary>
public struct K6_1_TestComponent : IComponent
{
    public int Value;
}

/// <summary>
/// K6.1 test fixture — a mod-origin system that declares no
/// writes/reads and unconditionally writes
/// <see cref="K6_1_TestComponent"/> during <c>Update</c>. The DEBUG
/// isolation guard catches the undeclared write and routes through
/// <see cref="IModFaultSink.ReportFault"/> before throwing
/// <see cref="IsolationViolationException"/>. The violation fires
/// before <c>World.SetComponent</c> is reached, so an arbitrary
/// <c>default</c> <see cref="EntityId"/> suffices.
/// </summary>
[SystemAccess(reads: new Type[0], writes: new Type[0], buses: new string[0])]
[TickRate(DualFrontier.Contracts.Attributes.TickRates.NORMAL)]
public sealed class ModFaultingSystem : K6_1_FaultingSystemBase
{
}

/// <summary>
/// K6.1 test fixture — same shape as <see cref="ModFaultingSystem"/>,
/// registered with <see cref="SystemOrigin.Core"/> in the test setup
/// to verify core systems do NOT route through
/// <see cref="IModFaultSink"/>. The same isolation violation throws
/// <see cref="IsolationViolationException"/> but leaves the handler
/// empty.
/// </summary>
[SystemAccess(reads: new Type[0], writes: new Type[0], buses: new string[0])]
[TickRate(DualFrontier.Contracts.Attributes.TickRates.NORMAL)]
public sealed class CoreFaultingSystem : K6_1_FaultingSystemBase
{
}

/// <summary>
/// K6.1 test fixture — a mod-origin system that declares a write
/// for <see cref="K6_1_TestComponent"/> and stays inside the
/// declaration. Used in the multi-system tests to verify the
/// metadata propagation is per-system and the non-faulting system's
/// modId does not enter the handler set. Uses
/// <c>default(EntityId)</c> so the call hits
/// <c>World.SetComponent</c> harmlessly with no isolation violation.
/// </summary>
[SystemAccess(reads: new Type[0], writes: new[] { typeof(K6_1_TestComponent) }, buses: new string[0])]
[TickRate(DualFrontier.Contracts.Attributes.TickRates.NORMAL)]
public sealed class ModBenignSystem : SystemBase
{
    public override void Update(float deltaSeconds)
    {
        // Empty: writing a component to a default EntityId would touch
        // the World; the test only needs this system NOT to fault.
    }
}

/// <summary>
/// K6.1 test fixture — a mod-origin system that throws the
/// isolation violation during <c>OnInitialize</c>. Used to verify the
/// fault sink wiring is active during the scheduler ctor's
/// <c>InitializeAllSystems</c> call (not just Update).
/// <c>SystemBase.Initialize</c> is internal and non-virtual, so the
/// override target is the <c>OnInitialize</c> hook.
/// </summary>
[SystemAccess(reads: new Type[0], writes: new Type[0], buses: new string[0])]
[TickRate(DualFrontier.Contracts.Attributes.TickRates.NORMAL)]
public sealed class ModFaultingInInitializeSystem : SystemBase
{
    protected override void OnInitialize()
    {
        K6_1_FaultingSystemBase.TriggerUndeclaredWrite(this);
    }

    public override void Update(float deltaSeconds)
    {
    }
}

/// <summary>
/// Shared base for the fault-trigger systems: an Update body that
/// commits an undeclared write of <see cref="K6_1_TestComponent"/>.
/// Lives outside the individual fixture classes so the same logic
/// can also be invoked from <c>OnInitialize</c> overrides.
/// </summary>
public abstract class K6_1_FaultingSystemBase : SystemBase
{
    public override void Update(float deltaSeconds)
    {
        TriggerUndeclaredWrite(this);
    }

    internal static void TriggerUndeclaredWrite(SystemBase _)
    {
        // Any active SystemExecutionContext on the calling thread will
        // serve — the scheduler has already pushed the per-system
        // context. The cast back through SystemExecutionContext.Current
        // bypasses SystemBase's protected SetComponent so the call
        // works even when invoked from a static helper.
        SystemExecutionContext.Current!
            .SetComponent(default(EntityId), new K6_1_TestComponent { Value = 1 });
    }
}
