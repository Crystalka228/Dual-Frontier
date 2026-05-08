using System;
using DualFrontier.Core.Interop.Marshalling;

namespace DualFrontier.Core.Interop;

/// <summary>
/// Static entry point for native engine bootstrap (K3, 2026-05-07).
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
/// </summary>
public static class Bootstrap
{
    /// <summary>
    /// Performs native engine bootstrap and returns a ready
    /// <see cref="NativeWorld"/>.
    /// </summary>
    /// <param name="registry">
    /// Optional <see cref="ComponentTypeRegistry"/>. If provided, the
    /// returned <see cref="NativeWorld"/> uses deterministic registry-based
    /// type ids. If null, the legacy FNV-1a fallback path is used (per K2).
    /// </param>
    /// <returns>
    /// Ready-to-use <see cref="NativeWorld"/> with active World handle, fully
    /// bootstrapped.
    /// </returns>
    /// <exception cref="BootstrapFailedException">
    /// If native bootstrap fails (memory allocation, thread spawn, or any
    /// task body throws). The native side performs full rollback before this
    /// exception propagates — no partial state remains.
    /// </exception>
    public static NativeWorld Run(ComponentTypeRegistry? registry = null)
    {
        IntPtr handle = NativeMethods.df_engine_bootstrap();
        if (handle == IntPtr.Zero)
        {
            throw new BootstrapFailedException(
                "df_engine_bootstrap returned null. Native side performed " +
                "full rollback. See native logs (when implemented) for the " +
                "specific task that failed.");
        }
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
