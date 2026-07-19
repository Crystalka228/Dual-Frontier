using System;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Contracts.Sdk;

/// <summary>
/// The per-tick capability surface handed to an <see cref="ISimulationSystem"/>.
/// The capability-scoped promotion of the engine-internal execution context:
/// it exposes exactly the measured system-to-world capability union
/// — component access, events, and simulation time — through Contracts-safe
/// forms, and NOTHING ELSE. No concrete <c>NativeWorld</c>, no
/// <c>Core.Interop</c> type, ever crosses this surface (audit A4: Contracts must
/// stay reference-free).
///
/// <para>
/// <b>Freshness (per-tick).</b> A system receives the context anew each tick;
/// it is a transient view, not a durable handle. Caching the context — or any
/// value obtained from it — in system state across ticks is FORBIDDEN: a cached
/// reference survives graph rebuilds and mod hot-reloads that invalidate it
/// (ECS.md §8, the "no engine references across ticks" anti-pattern). Values
/// read this tick are valid this tick.
/// </para>
///
/// <para>
/// <b>Deliberate omissions (day one).</b> There is NO field/compute surface and
/// NO managed-store accessor here: both have zero measured consumers, so they
/// are deferred rather than speculatively shipped (audience-driven deferral,
/// Lesson N17). Services are NOT on the per-tick context either — a system
/// receives its dependencies once at construction via
/// <see cref="ISystemServices"/>, not per tick. This surface grows only when a
/// measured consumer appears.
/// </para>
/// </summary>
public interface ISystemContext
{
    /// <summary>
    /// The current simulation tick (SimTick) — the monotonic counter advanced
    /// once per fixed 30 Hz step (TIME_AND_CONSISTENCY_MODEL §1;
    /// <c>TickScheduler.CurrentTick</c>). This is the ONLY temporal input: the
    /// contract carries no <c>float delta</c> (the fixed step is constant, and
    /// the measured harness reads no delta), and <c>[TickRate]</c> is a producer
    /// cadence, not a second clock.
    /// </summary>
    long CurrentTick { get; }

    // ---- Component access: per-id ----

    /// <summary>Reads the component of type <typeparamref name="T"/> on <paramref name="id"/>, if present.</summary>
    bool TryGetComponent<T>(EntityId id, out T value) where T : unmanaged, IComponent;

    /// <summary>True if <paramref name="id"/> carries a component of type <typeparamref name="T"/>.</summary>
    bool HasComponent<T>(EntityId id) where T : unmanaged, IComponent;

    /// <summary>Reads the component of type <typeparamref name="T"/> on <paramref name="id"/> (throws if absent).</summary>
    T GetComponent<T>(EntityId id) where T : unmanaged, IComponent;

    // ---- Component access: bulk ----

    /// <summary>
    /// Acquires a scoped, read-only span over all components of type
    /// <typeparamref name="T"/>. Use in a <c>using</c> scope; while it is live,
    /// mutations are rejected. Allocation-free (Path α, К-L3.1).
    /// </summary>
    SpanScope<T> AcquireSpan<T>() where T : unmanaged, IComponent;

    /// <summary>
    /// Begins a scoped write batch for components of type <typeparamref name="T"/>.
    /// Recorded commands apply atomically at <see cref="WriteScope{T}.Flush"/>
    /// (or on scope dispose). Allocation-free batched write (К-L3.1).
    /// </summary>
    WriteScope<T> BeginBatch<T>() where T : unmanaged, IComponent;

    // ---- String interning ----

    /// <summary>Interns <paramref name="content"/> and returns its handle.</summary>
    StringHandle InternString(string content);

    /// <summary>Resolves an interned handle back to its string content, or <c>null</c> if stale/empty.</summary>
    string? Resolve(StringHandle handle);

    // ---- Composites (per-entity variable-length lists) ----

    /// <summary>Allocates a fresh composite (one per component instance).</summary>
    CompositeHandle<T> CreateComposite<T>() where T : unmanaged;

    /// <summary>Appends <paramref name="value"/> to <paramref name="entity"/>'s list in the composite.</summary>
    bool CompositeAdd<T>(CompositeHandle<T> composite, EntityId entity, T value) where T : unmanaged;

    /// <summary>Reads the element at <paramref name="index"/> for <paramref name="entity"/>, if present.</summary>
    bool CompositeTryGetAt<T>(CompositeHandle<T> composite, EntityId entity, int index, out T value) where T : unmanaged;

    /// <summary>Number of elements <paramref name="entity"/> holds in the composite.</summary>
    int CompositeCountFor<T>(CompositeHandle<T> composite, EntityId entity) where T : unmanaged;

    /// <summary>Clears <paramref name="entity"/>'s list in the composite.</summary>
    bool CompositeClearFor<T>(CompositeHandle<T> composite, EntityId entity) where T : unmanaged;

    // ---- Events ----

    /// <summary>
    /// Publishes an event to the domain bus its type routes to. Capability-gated:
    /// an event the system's mod has not declared is rejected LOUDLY by the
    /// engine's existing capability enforcement (MOD_OS_ARCHITECTURE §3.6).
    /// </summary>
    void Publish<T>(T evt) where T : IEvent;

    /// <summary>
    /// Subscribes a handler to events of type <typeparamref name="T"/>. Same
    /// capability gating as <see cref="Publish{T}"/>; the subscription is tracked
    /// and released automatically when the mod unloads (DoD item 7).
    /// </summary>
    void Subscribe<T>(Action<T> handler) where T : IEvent;
}
