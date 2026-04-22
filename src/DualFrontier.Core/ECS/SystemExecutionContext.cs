using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Isolation guard for systems. When a specific system is executing,
/// <c>Current</c> holds its context: allowed READ/WRITE types,
/// system name (for error messages), reference to <c>World</c>,
/// and <c>SystemOrigin</c> — whether the system is Core or Mod.
///
/// Context is stored in <c>ThreadLocal{T}</c> — each scheduler thread
/// has its own. The scheduler sets the context before calling
/// <c>SystemBase.Update</c> and clears it after.
///
/// Violation reaction depends on <c>SystemOrigin</c>:
/// Core systems throw <c>IsolationViolationException</c>
/// (crash — this is a developer bug).
/// Mod systems call <c>IModFaultSink.ReportFault</c> and throw
/// <c>ModIsolationException</c>, which <c>ModFaultHandler</c> catches
/// and gracefully unloads the mod while the game continues.
///
/// WARNING: context breaks with async/await inside a system —
/// the continuation may land on a different thread where
/// <c>Current</c> is null. async is strictly forbidden in Domain.
/// See TechArch 11.7.
/// </summary>
public sealed class SystemExecutionContext
{
    // TODO: Phase 2 (Opus) — private static readonly ThreadLocal<SystemExecutionContext?> _current = new();
    // TODO: Phase 2 (Opus) — private readonly HashSet<Type> _allowedReads;
    // TODO: Phase 2 (Opus) — private readonly HashSet<Type> _allowedWrites;
    // TODO: Phase 2 (Opus) — private readonly string _systemName;
    // TODO: Phase 2 (Opus) — private readonly World _world;
    // TODO: Phase 2 (Opus) — private readonly SystemOrigin _systemOrigin;
    // TODO: Phase 2 (Opus) — private readonly string? _modId;

    /// <summary>
    /// Current execution context for the calling thread.
    /// Null if the thread does not belong to the scheduler
    /// (e.g. Godot main thread).
    /// TODO: Phase 2 (Opus) — implement via ThreadLocal.
    /// </summary>
    public static SystemExecutionContext? Current
    {
        get => throw new NotImplementedException("TODO: Phase 2 — ThreadLocal<SystemExecutionContext>");
    }

    /// <summary>
    /// Validated component access.
    /// In DEBUG: if <typeparamref name="T"/> is not in _allowedReads/_allowedWrites,
    /// throws <c>IsolationViolationException</c> for Core systems
    /// or <c>ModIsolationException</c> for mod systems.
    /// In RELEASE: direct call to <c>World.GetComponentUnsafe</c>.
    /// TODO: Phase 2 (Opus) — implement isolation check.
    /// </summary>
    public T GetComponent<T>(EntityId id) where T : IComponent
    {
        throw new NotImplementedException("TODO: Phase 2 — isolation check + delegate to World");
    }

    /// <summary>
    /// Always throws <c>IsolationViolationException</c>.
    /// Direct access to another system is forbidden — use EventBus instead.
    /// Method exists intentionally: attempting to call it produces a clear
    /// exception rather than a silent compile error.
    /// TODO: Phase 2 (Opus) — implement exception with helpful message.
    /// </summary>
    public TSystem GetSystem<TSystem>() where TSystem : class
    {
        throw new NotImplementedException("TODO: Phase 2 — always throw IsolationViolationException");
    }
}