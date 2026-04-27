using System;

namespace DualFrontier.Contracts.Attributes;

/// <summary>
/// Marks an event as "deferred". The bus accumulates such events during the
/// current phase and delivers them to subscribers in the next scheduler
/// phase. This is the safe mode for events that change world state (deaths,
/// entity removals) — they do not violate the invariants of the running
/// systems.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class DeferredAttribute : Attribute
{
}
