using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Isolation guard for systems. When a specific system is executing,
/// <see cref="Current"/> holds its context: allowed READ/WRITE types,
/// system name (for error messages), reference to {World}, and
/// {SystemOrigin} — whether the system is Core or Mod.
///
/// Context is stored in {ThreadLocal{T}} — each scheduler thread has its own.
/// The scheduler pushes the context before calling <c>SystemBase.Update</c>
/// and pops it after (in a <c>finally</c> block).
///
/// Violation reaction depends on <see cref="SystemOrigin"/>:
/// Core systems throw <see cref="IsolationViolationException"/> (crash —
/// this is a developer bug); Mod systems additionally route the diagnostic
/// through {IModFaultSink}, so the Application layer's {ModFaultHandler}
/// can unload the mod gracefully while the game continues running.
///
/// In Release builds, the access checks are elided by <c>#if DEBUG</c>,
/// making <see cref="GetComponent{T}"/>/<see cref="SetComponent{T}"/> a thin
/// pass-through with zero guard overhead. <see cref="GetSystem{TSystem}"/>
/// always throws regardless of build flavor — direct system-to-system
/// references are a semantic architectural violation.
///
/// WARNING: context breaks with async/await inside a system — the
/// continuation may land on a different thread where <see cref="Current"/>
/// is null. async/await is strictly forbidden in Domain. See TechArch 11.7.
/// </summary>
public sealed class SystemExecutionContext
{
    private static readonly ThreadLocal<SystemExecutionContext?> _current = new();

    private readonly World _world;
    private readonly string _systemName;
    private readonly HashSet<Type> _allowedReads;
    private readonly HashSet<Type> _allowedWrites;
    private readonly IReadOnlyList<string> _allowedBuses;
    private readonly SystemOrigin _origin;
    private readonly string? _modId;
    private readonly IModFaultSink _faultSink;

    /// <summary>
    /// Creates a guard for the given system. The scheduler (or a test)
    /// populates allowed reads/writes/buses from the system's
    /// <c>[SystemAccess]</c> declaration. {Origin} decides whether
    /// violations crash ({Core}) or route through {IModFaultSink} ({Mod}).
    /// {ModId} is only meaningful when <paramref name="origin"/> is
    /// <see cref="SystemOrigin.Mod"/>.
    /// </summary>
    /// <param name="world">Target world the guarded system may access.</param>
    /// <param name="systemName">Display name used in violation messages.</param>
    /// <param name="allowedReads">Component types the system may read.</param>
    /// <param name="allowedWrites">Component types the system may write.</param>
    /// <param name="allowedBuses">Bus names the system may publish to.</param>
    /// <param name="origin">Core vs Mod provenance of the system.</param>
    /// <param name="modId">Mod identifier when <paramref name="origin"/> is Mod; otherwise null.</param>
    /// <param name="faultSink">Destination for mod-origin fault reports.</param>
    internal SystemExecutionContext(
        World world,
        string systemName,
        IEnumerable<Type> allowedReads,
        IEnumerable<Type> allowedWrites,
        IEnumerable<string> allowedBuses,
        SystemOrigin origin,
        string? modId,
        IModFaultSink faultSink)
    {
        _world = world ?? throw new ArgumentNullException(nameof(world));
        _systemName = systemName ?? throw new ArgumentNullException(nameof(systemName));
        if (allowedReads is null) throw new ArgumentNullException(nameof(allowedReads));
        if (allowedWrites is null) throw new ArgumentNullException(nameof(allowedWrites));
        if (allowedBuses is null) throw new ArgumentNullException(nameof(allowedBuses));
        _faultSink = faultSink ?? throw new ArgumentNullException(nameof(faultSink));

        _allowedReads = new HashSet<Type>(allowedReads);
        _allowedWrites = new HashSet<Type>(allowedWrites);

        var buses = new List<string>();
        foreach (string bus in allowedBuses)
            buses.Add(bus);
        _allowedBuses = buses;

        _origin = origin;
        _modId = modId;
    }

    /// <summary>
    /// Current execution context for the calling thread. Null when the
    /// calling thread does not belong to the scheduler (e.g. Godot main
    /// thread, a test that has not pushed a context).
    /// </summary>
    public static SystemExecutionContext? Current => _current.Value;

    /// <summary>
    /// Pushes <paramref name="context"/> onto the calling thread's slot.
    /// Throws <see cref="InvalidOperationException"/> if a context is
    /// already set — indicates a scheduler bug (nested push without pop).
    /// Called by {ParallelSystemScheduler} before <c>SystemBase.Update</c>.
    /// </summary>
    /// <param name="context">The context to make current on this thread.</param>
    internal static void PushContext(SystemExecutionContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));
        if (_current.Value is not null)
            throw new InvalidOperationException(
                "SystemExecutionContext is already set on this thread — nested push detected. " +
                "This indicates a scheduler bug: PushContext was called without a matching PopContext.");
        _current.Value = context;
    }

    /// <summary>
    /// Pops the context from the calling thread's slot. Called by
    /// {ParallelSystemScheduler} in a <c>finally</c> block after
    /// <c>SystemBase.Update</c> so the slot is always cleared, even on
    /// exceptions.
    /// </summary>
    internal static void PopContext()
    {
        _current.Value = null;
    }

    /// <summary>
    /// Reads the component of type {T} from entity <paramref name="id"/>.
    /// In DEBUG: throws <see cref="IsolationViolationException"/> if {T}
    /// is not declared in the system's <c>[SystemAccess]</c> reads or
    /// writes. In RELEASE: the check is elided — thin pass-through to
    /// <c>World.GetComponentUnsafe</c>.
    /// </summary>
    /// <typeparam name="T">Component type to read.</typeparam>
    /// <param name="id">Target entity.</param>
    /// <returns>The stored component instance.</returns>
    public T GetComponent<T>(EntityId id) where T : IComponent
    {
#if DEBUG
        if (!IsReadAllowed(typeof(T)))
            ThrowUndeclaredRead(typeof(T));
#endif
        return _world.GetComponentUnsafe<T>(id);
    }

    /// <summary>
    /// Writes a component of type {T} on entity <paramref name="id"/>.
    /// In DEBUG: throws <see cref="IsolationViolationException"/> if {T}
    /// is not declared in the system's <c>[SystemAccess]</c> writes.
    /// In RELEASE: the check is elided — thin pass-through to
    /// <c>World.SetComponent</c>.
    /// </summary>
    /// <typeparam name="T">Component type to write.</typeparam>
    /// <param name="id">Target entity.</param>
    /// <param name="value">New component value.</param>
    public void SetComponent<T>(EntityId id, T value) where T : IComponent
    {
#if DEBUG
        if (!_allowedWrites.Contains(typeof(T)))
            ThrowUndeclaredWrite(typeof(T));
#endif
        _world.SetComponent<T>(id, value);
    }

    /// <summary>
    /// Enumerates entities that currently have a component of type {T}.
    /// Lazy — uses <c>yield return</c> so iteration stops at the caller's
    /// <c>break</c> without materialising. In DEBUG: throws
    /// <see cref="IsolationViolationException"/> if {T} is not declared
    /// in the system's <c>[SystemAccess]</c> reads or writes.
    /// </summary>
    /// <typeparam name="T">Component type whose owners to enumerate.</typeparam>
    /// <returns>Lazy sequence of entity ids carrying a component of type {T}.</returns>
    public IEnumerable<EntityId> Query<T>() where T : IComponent
    {
#if DEBUG
        if (!IsReadAllowed(typeof(T)))
            ThrowUndeclaredRead(typeof(T));
#endif
        return _world.GetEntitiesWith<T>();
    }

    /// <summary>
    /// Enumerates entities that currently have both a {T1} and a {T2}
    /// component. Picks the smaller store to iterate and filters by
    /// <c>HasComponent</c> on the larger one. Lazy — no materialisation.
    /// In DEBUG: each type parameter is checked against declared reads
    /// or writes.
    /// </summary>
    /// <typeparam name="T1">First required component type.</typeparam>
    /// <typeparam name="T2">Second required component type.</typeparam>
    /// <returns>Lazy sequence of entity ids carrying both components.</returns>
    public IEnumerable<EntityId> Query<T1, T2>()
        where T1 : IComponent
        where T2 : IComponent
    {
#if DEBUG
        if (!IsReadAllowed(typeof(T1)))
            ThrowUndeclaredRead(typeof(T1));
        if (!IsReadAllowed(typeof(T2)))
            ThrowUndeclaredRead(typeof(T2));
#endif
        return QueryIntersection<T1, T2>();
    }

    private IEnumerable<EntityId> QueryIntersection<T1, T2>()
        where T1 : IComponent
        where T2 : IComponent
    {
        int count1 = _world.GetComponentCount<T1>();
        int count2 = _world.GetComponentCount<T2>();

        if (count1 <= count2)
        {
            foreach (EntityId id in _world.GetEntitiesWith<T1>())
            {
                if (_world.HasComponent<T2>(id))
                    yield return id;
            }
        }
        else
        {
            foreach (EntityId id in _world.GetEntitiesWith<T2>())
            {
                if (_world.HasComponent<T1>(id))
                    yield return id;
            }
        }
    }

    /// <summary>
    /// Always throws <see cref="IsolationViolationException"/>. Direct
    /// system-to-system references break the architecture's isolation
    /// rules — use {EventBus} instead. This method exists intentionally
    /// so that attempting to reach a sibling system produces a clear,
    /// actionable diagnostic rather than a silent compile error.
    /// </summary>
    /// <typeparam name="TSystem">System type the caller tried to resolve.</typeparam>
    /// <returns>Never returns — always throws.</returns>
    public TSystem GetSystem<TSystem>() where TSystem : SystemBase
    {
        throw new IsolationViolationException(
            "[IsolationViolationException]" + Environment.NewLine +
            "Прямой доступ к системам запрещён." + Environment.NewLine +
            "Используй EventBus вместо прямой ссылки на систему.");
    }

    private bool IsReadAllowed(Type componentType)
    {
        return _allowedReads.Contains(componentType)
            || _allowedWrites.Contains(componentType);
    }

    private void ThrowUndeclaredRead(Type componentType)
    {
        string message = BuildReadViolationMessage(componentType);
        RouteAndThrow(message);
    }

    private void ThrowUndeclaredWrite(Type componentType)
    {
        string message = BuildWriteViolationMessage(componentType);
        RouteAndThrow(message);
    }

    private string BuildReadViolationMessage(Type componentType)
    {
        var sb = new StringBuilder();
        sb.Append("[ИЗОЛЯЦИЯ НАРУШЕНА]").Append(Environment.NewLine);
        sb.Append("Система '").Append(_systemName).Append("'").Append(Environment.NewLine);
        sb.Append("обратилась к '").Append(componentType.Name).Append("'").Append(Environment.NewLine);
        sb.Append("без декларации доступа.").Append(Environment.NewLine);
        sb.Append("Добавь: [SystemAccess(reads: new[]{typeof(")
          .Append(componentType.Name).Append(")})]");
        return sb.ToString();
    }

    private string BuildWriteViolationMessage(Type componentType)
    {
        var sb = new StringBuilder();
        sb.Append("[ИЗОЛЯЦИЯ НАРУШЕНА]").Append(Environment.NewLine);
        sb.Append("Система '").Append(_systemName).Append("'").Append(Environment.NewLine);
        sb.Append("модифицирует '").Append(componentType.Name).Append("'").Append(Environment.NewLine);
        sb.Append("без декларации записи.").Append(Environment.NewLine);
        sb.Append("Добавь: [SystemAccess(writes: new[]{typeof(")
          .Append(componentType.Name).Append(")})]");
        return sb.ToString();
    }

    private void RouteAndThrow(string message)
    {
        if (_origin == SystemOrigin.Mod)
            _faultSink.ReportFault(_modId!, message);
        throw new IsolationViolationException(message);
    }
}
