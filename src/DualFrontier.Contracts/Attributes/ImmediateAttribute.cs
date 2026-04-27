using System;

namespace DualFrontier.Contracts.Attributes;

/// <summary>
/// Marks an event as "immediate" — interrupts the current scheduler phase
/// for direct delivery to subscribers. Used sparingly: only for critical
/// events that cannot be deferred (e.g. a game event that requires an
/// immediate pause). Excessive use breaks parallelism.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class ImmediateAttribute : Attribute
{
}
