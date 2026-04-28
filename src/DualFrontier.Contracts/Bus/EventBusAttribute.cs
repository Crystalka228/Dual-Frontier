using System;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Marks an <see cref="DualFrontier.Contracts.Core.IEvent"/> record with
/// the name of the <see cref="IGameServices"/> property that owns its bus.
/// Used by <c>ModBusRouter</c> and by <c>RestrictedModApi</c> to route
/// mod-published events to the correct domain bus without hardcoding the
/// mapping.
///
/// Example:
/// <code>
/// [EventBus("Combat")]
/// [Deferred]
/// public sealed record DamageEvent(...) : IEvent;
/// </code>
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class EventBusAttribute : Attribute
{
    /// <summary>
    /// Name of the property on <see cref="IGameServices"/> that returns
    /// the bus this event belongs to (e.g. <c>"Combat"</c>).
    /// </summary>
    public string BusName { get; }

    /// <summary>
    /// Creates an attribute binding the decorated event type to the bus
    /// exposed by <see cref="IGameServices"/> under <paramref name="busName"/>.
    /// </summary>
    /// <param name="busName">Property name on <see cref="IGameServices"/>.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="busName"/> is null, empty, or whitespace.
    /// </exception>
    public EventBusAttribute(string busName)
    {
        if (string.IsNullOrWhiteSpace(busName))
            throw new ArgumentException("BusName must not be empty.", nameof(busName));
        BusName = busName;
    }
}
