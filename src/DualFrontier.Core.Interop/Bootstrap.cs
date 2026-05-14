using System;
using DualFrontier.Core.Interop.Marshalling;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Static entry point for native engine bootstrap (K3, 2026-05-07; refactored
/// K8.3+K8.4 combined milestone, 2026-05-14).
///
/// Calls the native bootstrap graph which initializes the World + an
/// internal thread pool in parallel, returning a ready-to-use
/// <see cref="NativeWorld"/> instance.
///
/// Per K-L5 + Q1-Q4 architectural decisions:
/// <list type="bullet">
///   <item>Bootstrap is a single-call atomic (либо fully ready либо exception).</item>
///   <item>No partial visibility — failure modes throw <see cref="BootstrapFailedException"/>,
///         never return a half-initialized <see cref="NativeWorld"/>.</item>
///   <item>The native thread pool is internal and ephemeral; it is destroyed
///         after bootstrap completes per Q1 (Minimal scope). The game tick
///         remains managed (K-L6).</item>
/// </list>
///
/// K8.3+K8.4 combined milestone refactor: the K3-era overload accepting an
/// already-constructed <see cref="ComponentTypeRegistry"/> was unreachable from
/// production (the registry's only constructor requires the world handle, which
/// only exists post-bootstrap — chicken-and-egg). The registry is now constructed
/// inside <see cref="Run"/> against the bootstrapped handle, via the same-
/// assembly internal constructor. K-L4 deterministic sequential type ids are
/// the production default; the FNV-1a fallback path is preserved via
/// <c>useRegistry: false</c> for diagnostic and legacy test scenarios.
/// </summary>
public static class Bootstrap
{
    /// <summary>
    /// Performs native engine bootstrap and returns a ready
    /// <see cref="NativeWorld"/>.
    /// </summary>
    /// <param name="useRegistry">
    /// When true (default, K8.3+K8.4 production path), a <see cref="ComponentTypeRegistry"/>
    /// is constructed against the bootstrapped world handle and bound to the
    /// returned <see cref="NativeWorld"/> — deterministic sequential type ids
    /// per K-L4. When false, the legacy FNV-1a fallback path is used (retained
    /// for tests that exercise the registry class in isolation by constructing
    /// their own <see cref="ComponentTypeRegistry"/> against the same handle;
    /// no production caller passes false post-K8.3+K8.4).
    /// </param>
    /// <returns>
    /// Ready-to-use <see cref="NativeWorld"/> with an active World handle, fully
    /// bootstrapped. When <paramref name="useRegistry"/> is true,
    /// <see cref="NativeWorld.Registry"/> is non-null and ready for
    /// <c>VanillaComponentRegistration.RegisterAll</c> (defined in
    /// DualFrontier.Application — cannot be linked here as Core.Interop sits
    /// below Application in the assembly graph).
    /// </returns>
    /// <exception cref="BootstrapFailedException">
    /// If native bootstrap fails (memory allocation, thread spawn, or any
    /// task body throws). The native side performs full rollback before this
    /// exception propagates — no partial state remains.
    /// </exception>
    public static NativeWorld Run(bool useRegistry = true)
    {
        IntPtr handle = NativeMethods.df_engine_bootstrap();
        if (handle == IntPtr.Zero)
        {
            throw new BootstrapFailedException(
                "df_engine_bootstrap returned null. Native side performed " +
                "full rollback. See native logs (when implemented) for the " +
                "specific task that failed.");
        }

        // Registry construction lives here — the single point where the handle
        // exists but the world is not yet handed to callers. The internal ctor
        // is legally reachable: Bootstrap and ComponentTypeRegistry share the
        // DualFrontier.Core.Interop assembly. K-L5 atomicity preserved — the
        // world is either fully ready (handle + bound registry) or an exception
        // propagated.
        ComponentTypeRegistry? registry =
            useRegistry ? new ComponentTypeRegistry(handle) : null;

        return NativeWorld.AdoptBootstrappedHandle(handle, registry);
    }
}

/// <summary>
/// Thrown when native engine bootstrap fails. The native side has already
/// performed full rollback by the time this exception propagates — no
/// partial state remains to clean up.
/// </summary>
public sealed class BootstrapFailedException : Exception
{
    public BootstrapFailedException(string message) : base(message) { }
    public BootstrapFailedException(string message, Exception inner)
        : base(message, inner) { }
}
