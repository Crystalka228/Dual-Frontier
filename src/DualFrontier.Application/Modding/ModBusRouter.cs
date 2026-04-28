using System;
using System.Collections.Generic;
using System.Reflection;
using DualFrontier.Contracts.Bus;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Maps an event type carrying <see cref="EventBusAttribute"/> to the bus
/// instance that owns it, by reflecting over <see cref="IGameServices"/>.
/// The property lookup is built once at first access and cached.
/// </summary>
internal static class ModBusRouter
{
    // Key: BusName matched against the IGameServices property name.
    // Comparison is ordinal because IGameServices uses PascalCase property
    // names and the rest of the modding subsystem is case-sensitive.
    private static readonly IReadOnlyDictionary<string, PropertyInfo> _busProperties = BuildMap();

    /// <summary>
    /// Returns the bus instance for <paramref name="eventType"/>, or
    /// <see langword="null"/> when the event type carries no
    /// <see cref="EventBusAttribute"/> or its <c>BusName</c> does not
    /// correspond to a property on <see cref="IGameServices"/>.
    /// </summary>
    /// <param name="eventType">The event type whose bus to resolve.</param>
    /// <param name="services">The aggregator of all domain buses.</param>
    internal static object? Resolve(Type eventType, IGameServices services)
    {
        if (eventType is null) throw new ArgumentNullException(nameof(eventType));
        if (services is null) throw new ArgumentNullException(nameof(services));

        EventBusAttribute? attr = eventType.GetCustomAttribute<EventBusAttribute>(inherit: false);
        if (attr is null) return null;
        if (!_busProperties.TryGetValue(attr.BusName, out PropertyInfo? prop)) return null;
        return prop.GetValue(services);
    }

    private static Dictionary<string, PropertyInfo> BuildMap()
    {
        var map = new Dictionary<string, PropertyInfo>(StringComparer.Ordinal);
        PropertyInfo[] props = typeof(IGameServices).GetProperties(
            BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo prop in props)
            map.Add(prop.Name, prop);
        return map;
    }
}
