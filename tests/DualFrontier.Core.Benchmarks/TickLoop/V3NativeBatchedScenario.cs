using System;
using DualFrontier.Components.Building;
using DualFrontier.Components.Items;
using DualFrontier.Components.Pawn;
using DualFrontier.Components.Shared;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;
using DualFrontier.Core.Interop.Marshalling;

namespace DualFrontier.Core.Benchmarks.TickLoop;

/// <summary>
/// K7 V3 scenario — <see cref="NativeWorld"/> + struct components +
/// <see cref="SpanLease{T}"/> reads + <see cref="WriteBatch{T}"/> writes.
///
/// Per K7 brief §1.8, the V3 system harness is purpose-written for the
/// NativeWorld access pattern. It does NOT exercise the production
/// <see cref="DualFrontier.Core.Scheduling.ParallelSystemScheduler"/>
/// because production systems are bound to the managed
/// <see cref="DualFrontier.Core.ECS.World"/>; running them on
/// <see cref="NativeWorld"/> would require the K8 cutover work, which is
/// out of K7 scope.
///
/// Workload coverage divergence vs V2 (LOCKED, documented in report):
///
/// 1. Class-based components (MovementComponent, IdentityComponent,
///    SkillsComponent, SocialComponent, StorageComponent,
///    WorkbenchComponent) cannot live in <see cref="NativeWorld"/>
///    because <c>unmanaged</c> excludes reference types. V3 therefore
///    omits the systems that read/write those (MovementSystem,
///    InventorySystem, HaulSystem, PawnStateReporterSystem). The V3
///    workload exercises only struct-component pipelines.
///
/// 2. V3 systems run every tick (no TickRate gating). Production V2
///    gates NeedsSystem/MoodSystem behind SLOW/NORMAL TickRates so
///    they fire less often than every tick. V3 numbers therefore
///    over-represent per-tick cost for systems whose production
///    counterparts run less often. The report calls this out.
///
/// 3. V3 does not publish NeedsCriticalEvent / NeedsRestoredEvent /
///    DeathEvent because the cross-system event flow uses the managed
///    bus aggregator (<see cref="DualFrontier.Core.Bus.GameServices"/>),
///    which is bound to the managed scheduler. V3 measures the storage
///    cost of the read/write pattern, not the full event-driven
///    pipeline.
///
/// What V3 DOES exercise:
/// - <see cref="NativeWorld"/> entity creation + bulk component add
/// - <see cref="ComponentTypeRegistry"/> deterministic id resolution
/// - <see cref="SpanLease{T}"/> dense-storage read iteration
/// - <see cref="WriteBatch{T}"/> command-buffer write batching
/// - Per-tick allocation cost of the lease + batch lifecycle
/// - Flush-time application of recorded commands
///
/// These are the access patterns K8 cutover would adopt for production
/// systems on NativeWorld. K7 measures their per-tick cost on a
/// production-shaped workload size (50 pawns + 255 items) so K8 has
/// evidence for the cutover decision.
/// </summary>
internal sealed class V3NativeBatchedScenario : TickLoopScenarioBase
{
    private const int InitialFoodCount = 150;
    private const int InitialWaterCount = 50;
    private const int InitialBedCount = 30;
    private const int InitialDecorationCount = 25;

    private NativeWorld _world = null!;

    public override void SetupWorld(int pawnCount, int seed)
    {
        // K2 explicit-registration path requires the registry ctor to
        // bind to an existing world handle, which is internal-only.
        // The FNV-1a fallback path is the supported public surface for
        // benchmarks; type ids are still stable per-process which is
        // all the benchmark needs.
        _world = new NativeWorld();

        var rng = new Random(seed);

        // Spawn pawns — same shape as RandomPawnFactory's struct-only
        // contribution. Class-based components (Movement, Identity,
        // Skills) are intentionally omitted per the divergence note.
        var pawnIds = new EntityId[pawnCount];
        var pawnNeeds = new NeedsComponent[pawnCount];
        var pawnMinds = new MindComponent[pawnCount];
        var pawnJobs = new JobComponent[pawnCount];
        var pawnHealths = new HealthComponent[pawnCount];
        var pawnPositions = new PositionComponent[pawnCount];
        var pawnRaces = new RaceComponent[pawnCount];

        for (int i = 0; i < pawnCount; i++)
        {
            pawnIds[i] = _world.CreateEntity();
            pawnNeeds[i] = new NeedsComponent
            {
                Satiety = 0.9f, Hydration = 0.9f, Sleep = 0.9f, Comfort = 1.0f,
            };
            pawnMinds[i] = new MindComponent { Mood = 0.5f };
            pawnJobs[i] = new JobComponent { Current = JobKind.Idle };
            pawnHealths[i] = new HealthComponent { Current = 100f, Maximum = 100f };
            pawnPositions[i] = new PositionComponent
            {
                Position = new DualFrontier.Contracts.Math.GridVector(
                    rng.Next(0, 200), rng.Next(0, 200)),
            };
            pawnRaces[i] = new RaceComponent { Kind = RaceKind.Human };
        }

        _world.AddComponents<NeedsComponent>(pawnIds, pawnNeeds);
        _world.AddComponents<MindComponent>(pawnIds, pawnMinds);
        _world.AddComponents<JobComponent>(pawnIds, pawnJobs);
        _world.AddComponents<HealthComponent>(pawnIds, pawnHealths);
        _world.AddComponents<PositionComponent>(pawnIds, pawnPositions);
        _world.AddComponents<RaceComponent>(pawnIds, pawnRaces);

        // World items — 255 entities with item-kind component + position.
        // Mirrors ItemFactory output sizes to keep workload shape parity
        // with V2.
        SpawnItems<ConsumableComponent>(rng, InitialFoodCount,
            () => new ConsumableComponent());
        SpawnItems<WaterSourceComponent>(rng, InitialWaterCount,
            () => new WaterSourceComponent());
        SpawnItems<BedComponent>(rng, InitialBedCount,
            () => new BedComponent());
        SpawnItems<DecorativeAuraComponent>(rng, InitialDecorationCount,
            () => new DecorativeAuraComponent());

        // K8.3+K8.4 cutover: power producers/consumers are gone with the
        // Power subsystem; V3PowerSystem benchmark step is a no-op now.
    }

    private void SpawnItems<T>(Random rng, int count, Func<T> factory) where T : unmanaged, IComponent
    {
        var ids = new EntityId[count];
        var items = new T[count];
        var positions = new PositionComponent[count];
        for (int i = 0; i < count; i++)
        {
            ids[i] = _world.CreateEntity();
            items[i] = factory();
            positions[i] = new PositionComponent
            {
                Position = new DualFrontier.Contracts.Math.GridVector(
                    rng.Next(0, 200), rng.Next(0, 200)),
            };
        }
        _world.AddComponents<T>(ids, items);
        _world.AddComponents<PositionComponent>(ids, positions);
    }

    public override void ExecuteTick(float delta)
    {
        // Order mirrors a typical phase ordering — depletion → reaction →
        // movement → external systems. Each system is a self-contained
        // SpanLease+WriteBatch pair, mirroring the post-K5 access pattern.
        TickNeedsDepletion(delta);
        TickMoodFromNeeds();
    }

    private void TickNeedsDepletion(float delta)
    {
        // Mirrors NeedsSystem.Update — deplete each need by a per-tick
        // constant. Production NeedsSystem runs at TickRates.SLOW
        // (~every 60 ticks); V3 runs every tick per the divergence note,
        // which over-represents this system's per-tick cost.
        using var batch = _world.BeginBatch<NeedsComponent>();
        using var lease = _world.AcquireSpan<NeedsComponent>();
        ReadOnlySpan<NeedsComponent> span = lease.Span;
        ReadOnlySpan<int> indices = lease.Indices;
        for (int i = 0; i < lease.Count; i++)
        {
            NeedsComponent updated = span[i];
            updated.Satiety   = System.Math.Clamp(updated.Satiety   - 0.0001f, 0f, 1f);
            updated.Hydration = System.Math.Clamp(updated.Hydration - 0.00015f, 0f, 1f);
            updated.Sleep     = System.Math.Clamp(updated.Sleep     - 0.00005f, 0f, 1f);
            updated.Comfort   = System.Math.Clamp(updated.Comfort   - 0.00002f, 0f, 1f);
            // EntityId reconstruction from the parallel index array.
            // Version is unknown to the lease; flush validates liveness
            // via the stored version side-band per K5 design.
            batch.Update(new EntityId(indices[i], 1), updated);
        }
    }

    private void TickMoodFromNeeds()
    {
        // Mirrors MoodSystem — derive mood from current needs. Reads
        // NeedsComponent (lease 1), reads MindComponent dense storage,
        // writes MindComponent via batch. Two concurrent leases on
        // different types is an explicit K1/K5 supported pattern.
        using var batch = _world.BeginBatch<MindComponent>();
        using var needsLease = _world.AcquireSpan<NeedsComponent>();
        using var mindLease = _world.AcquireSpan<MindComponent>();

        ReadOnlySpan<NeedsComponent> needs = needsLease.Span;
        ReadOnlySpan<MindComponent> minds = mindLease.Span;
        ReadOnlySpan<int> indices = mindLease.Indices;

        // V3 simplification: NeedsComponent and MindComponent both belong
        // to the same set of entities (every pawn carries both), and the
        // dense order matches because both were bulk-added in lockstep
        // during SetupWorld. Production code would resolve via entity
        // joins; the benchmark trades that for index parallelism.
        int n = System.Math.Min(minds.Length, needs.Length);
        for (int i = 0; i < n; i++)
        {
            float avgNeed = (needs[i].Satiety + needs[i].Hydration
                           + needs[i].Sleep + needs[i].Comfort) * 0.25f;
            MindComponent updated = minds[i];
            updated.Mood = System.Math.Clamp(avgNeed, 0f, 1f);
            batch.Update(new EntityId(indices[i], 1), updated);
        }
    }

    // TickPowerConsumption removed — Power subsystem deleted in K8.3+K8.4 cutover.

    public override void TeardownWorld()
    {
        _world?.Dispose();
        _world = null!;
    }
}
