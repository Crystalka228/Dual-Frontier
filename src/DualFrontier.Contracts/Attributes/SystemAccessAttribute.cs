using System;

namespace DualFrontier.Contracts.Attributes;

/// <summary>
/// Declares the system's access boundaries: which component types it reads and which it
/// writes. The managed <c>DependencyGraph</c> consumes Reads/Writes for edge-building (the
/// native <c>system_graph</c> mirrors the same edge semantics). The К8.3+К8.4 runtime
/// isolation guard that consumed it for per-access checks is deleted; W2/BD-3 retired the
/// bus leg (the former <c>Bus</c>/<c>Buses</c> members) with the genre taxonomy that left
/// the engine contract -- bus-publish scoping, if ever wanted, is future design on the FQN
/// capability gate, not a revival. Enforcement is compile-time. See
/// <c>docs/architecture/THREADING.md</c> and <c>docs/architecture/MOD_OS_ARCHITECTURE.md</c>.
///
/// The attribute is not inheritable — every system must declare its contract explicitly.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SystemAccessAttribute : Attribute
{
    /// <summary>
    /// Component types the system is allowed to read (post-К8.3+К8.4: through the
    /// <c>NativeWorld</c> span/batch surface — the former
    /// <c>SystemBase.GetComponent&lt;T&gt;</c> path is deleted).
    /// </summary>
    public Type[] Reads { get; }

    /// <summary>
    /// Component types the system is allowed to modify.
    /// Write implicitly grants Read.
    /// </summary>
    public Type[] Writes { get; }

    /// <summary>
    /// Creates the attribute with explicit reads and writes. W2/BD-3 removed the bus
    /// argument (the five-bus genre taxonomy + <c>IGameServices</c> left the engine
    /// contract): a system's event publication is no longer declared through this attribute.
    /// </summary>
    public SystemAccessAttribute(Type[] reads, Type[] writes)
    {
        Reads = reads ?? Array.Empty<Type>();
        Writes = writes ?? Array.Empty<Type>();
    }
}
