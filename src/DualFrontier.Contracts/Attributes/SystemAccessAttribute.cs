using System;

namespace DualFrontier.Contracts.Attributes;

/// <summary>
/// Declares the system's access boundaries: which component types it reads,
/// which it writes, and which domain bus it publishes events on.
/// <c>ParallelSystemScheduler</c> uses this declaration to build the
/// dependency graph, while <c>SystemExecutionContext</c> (the isolation
/// guard) uses it for DEBUG-build checks. See <c>/docs/THREADING.md</c>
/// and <c>/docs/ISOLATION.md</c> (TechArch sections 11.6 and 11.7).
///
/// The attribute is not inheritable — every system must declare its
/// contract explicitly.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SystemAccessAttribute : Attribute
{
    /// <summary>
    /// Component types the system is allowed to read through
    /// <c>SystemBase.GetComponent&lt;T&gt;</c>.
    /// </summary>
    public Type[] Reads { get; }

    /// <summary>
    /// Component types the system is allowed to modify.
    /// Write implicitly grants Read.
    /// </summary>
    public Type[] Writes { get; }

    /// <summary>
    /// Name of the domain bus from <c>IGameServices</c> the system publishes
    /// to. Use <c>nameof(IGameServices.Combat)</c> and similar for
    /// compile-time name checks. For multi-bus systems — the first bus from
    /// <see cref="Buses"/>.
    /// </summary>
    public string Bus { get; }

    /// <summary>
    /// Full list of buses the system writes to or reads from.
    /// Introduced in TechArch v0.2 §12.4: CombatSystem now operates on
    /// <c>Combat</c> and <c>Magic</c> simultaneously (mana arrow).
    /// For single-bus systems, contains a single element.
    /// </summary>
    public string[] Buses { get; }

    /// <summary>
    /// Creates the attribute with explicit reads/writes and a single bus.
    /// Compatible with the v0.1-style declaration.
    /// </summary>
    public SystemAccessAttribute(Type[] reads, Type[] writes, string bus)
    {
        Reads = reads ?? Array.Empty<Type>();
        Writes = writes ?? Array.Empty<Type>();
        Bus = bus ?? string.Empty;
        Buses = string.IsNullOrEmpty(Bus) ? Array.Empty<string>() : new[] { Bus };
    }

    /// <summary>
    /// Creates the attribute with explicit reads/writes and a list of buses.
    /// Introduced in TechArch v0.2 §12.4 for multi-bus systems
    /// (<c>CombatSystem</c>, <c>CompositeResolutionSystem</c>, <c>ComboResolutionSystem</c>).
    /// </summary>
    public SystemAccessAttribute(Type[] reads, Type[] writes, string[] buses)
    {
        Reads = reads ?? Array.Empty<Type>();
        Writes = writes ?? Array.Empty<Type>();
        Buses = buses ?? Array.Empty<string>();
        Bus = Buses.Length > 0 ? Buses[0] : string.Empty;
    }
}
