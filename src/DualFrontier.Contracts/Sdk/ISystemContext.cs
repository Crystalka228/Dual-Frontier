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

    // ---- Entity lifecycle ----

    /// <summary>
    /// Mints a live entity in the simulation world and returns its identity.
    /// Engine-generic: the new entity carries no components until the system
    /// attaches them (<see cref="BeginBatch{T}"/> then <c>Add</c>), and this
    /// surface vends no notion of what the entity "is" — that is mod data.
    ///
    /// <para>
    /// <b>Ids outlive the tick.</b> An <see cref="EntityId"/> MAY be held by a
    /// mod across ticks. It is WORLD IDENTITY — a generational handle the world
    /// resolves — not an engine reference. The freshness law above forbids
    /// caching the CONTEXT and the engine objects reached through it; the
    /// ECS.md §8 "no engine references across ticks" anti-pattern binds engine
    /// OBJECT references, not identities. A held id may of course name an
    /// entity that has since died — probe it with <see cref="IsEntityAlive"/>.
    /// </para>
    /// </summary>
    EntityId CreateEntity();

    /// <summary>
    /// Destroys <paramref name="id"/>. Liveness ends at once —
    /// <see cref="IsEntityAlive"/> reads <see langword="false"/> on the very next
    /// call — while the entity's COMPONENT STORAGE is reclaimed later, on the
    /// engine's native deferred-destroy flush at a tick boundary it owns. A bulk
    /// read taken between the two may still see the dead entity's component row;
    /// gate such reads on <see cref="IsEntityAlive"/> when it matters.
    ///
    /// <para>
    /// <b>Precondition: no live borrow.</b> Release every <see cref="SpanScope{T}"/> and
    /// <see cref="WriteScope{T}"/> before calling. The engine rejects a destroy while
    /// the world is borrowed, so the natural-looking "iterate a span and destroy as
    /// you go" shape does not work: read first, close the scope, then destroy. A live
    /// SPAN is detected and reported loudly; a live write BATCH is not currently
    /// detectable, so that case is a contract you keep, not one the engine checks.
    /// </para>
    ///
    /// <para>
    /// There is deliberately NO flush member on this surface. Flushing has
    /// whole-world ordering consequences, and a mod able to force one could tear
    /// component storage out from under a concurrently-running system. A mod
    /// states the intent; the engine schedules the reclamation.
    /// </para>
    /// </summary>
    void DestroyEntity(EntityId id);

    /// <summary>
    /// Liveness probe for <paramref name="id"/>. Reads <see langword="false"/>
    /// as soon as <see cref="DestroyEntity"/> is called — liveness flips
    /// immediately, it does not wait for the flush — and for any id whose
    /// generation is stale, so a recycled slot never reads as the entity that
    /// previously held it.
    /// </summary>
    bool IsEntityAlive(EntityId id);

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

    // ---- Presentation ----

    /// <summary>
    /// Modulates the whole rendered scene toward the colour
    /// (<paramref name="r"/>, <paramref name="g"/>, <paramref name="b"/>) by
    /// <paramref name="strength"/>. Channels and strength are 0..1;
    /// <paramref name="strength"/> 0 means no tint and restores the untinted
    /// scene exactly.
    ///
    /// <para>
    /// Engine-generic: this carries a COLOUR, not a meaning. The engine has no
    /// idea whether the mod is painting a storm, a nightfall, or a damage flash —
    /// that interpretation lives entirely in the mod.
    /// </para>
    ///
    /// <para>
    /// The call crosses to the renderer through the engine's presentation bridge,
    /// so it is safe from a system Tick and from an event handler alike; it needs
    /// no world access. If the host installed no presentation sink, the call
    /// throws rather than silently doing nothing (K-L19 fail-fast) — a mod whose
    /// visuals vanish without a diagnostic is the shape being prevented.
    /// </para>
    ///
    /// <para>
    /// <b>Planned</b> — the full layer/slot presentation model (BD-9) supersedes
    /// this single primitive and ABSORBS this member; see ROADMAP.md.
    /// </para>
    /// </summary>
    void SetAmbientTint(float r, float g, float b, float strength);

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
