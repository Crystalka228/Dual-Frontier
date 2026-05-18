using System;

namespace DualFrontier.Contracts.Scheduling;

/// <summary>
/// K10.1 К-L13 wake type enum. Stable values mirror the native
/// <c>dualfrontier::WakeRegistry::WakeType</c> enum for C ABI parity.
/// </summary>
public enum WakeType
{
    /// <summary>Periodic by <see cref="DualFrontier.Contracts.Attributes.TickRateAttribute"/>.</summary>
    Timer       = 0,
    /// <summary>Bus event publication subscription (K10.2 bus integration).</summary>
    Event       = 1,
    /// <summary>Component value condition crossing (Item 17 write-through hook).</summary>
    StateChange = 2,
    /// <summary>One-shot at startup, after SignalEngineReady.</summary>
    Init        = 3,
    /// <summary>API-driven wake by another system (scheduler.WakeSystem).</summary>
    Explicit    = 4,
}

/// <summary>
/// K10.1 Item 3 (К-L13) — Subscribes the system к bus events of the given type.
/// Native scheduler tracks the subscription; bus publication triggers wake
/// dispatch (К10.2 bus integration delivers payloads — К10.1 stores subscription
/// metadata only).
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class WakeOnEventAttribute : Attribute
{
    /// <summary>Event payload type (mapped к event_type_id at registration).</summary>
    public Type EventType { get; }

    /// <summary>Create the attribute for the given event payload type.</summary>
    public WakeOnEventAttribute(Type eventType)
    {
        EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
    }
}

/// <summary>
/// K10.1 Item 3 (К-L13) — Subscribes the system к state changes on a given
/// component type. The native write-through hook (Item 17) checks subscribed
/// conditions at <c>WriteBatch&lt;T&gt;.Commit()</c> time; on crossing,
/// the system is added к the runqueue для the next phase.
///
/// K10.1 stores the type-wide subscription only; per-entity condition evaluation
/// extends в К10.2 once condition expressions cross the C ABI cleanly.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class WakeOnStateAttribute : Attribute
{
    /// <summary>Component type whose value changes drive the wake.</summary>
    public Type ComponentType { get; }

    /// <summary>Create the attribute for the given component type.</summary>
    public WakeOnStateAttribute(Type componentType)
    {
        ComponentType = componentType ?? throw new ArgumentNullException(nameof(componentType));
    }
}

/// <summary>
/// K10.1 Item 3 (К-L13) — Marks the system as a one-shot Init wake subscriber.
/// The system runs once after <c>SignalEngineReady</c> completes and never
/// again until next bootstrap. Useful for initialization-only systems.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class WakeOnInitAttribute : Attribute
{
}

/// <summary>
/// K10.1 Item 3 (К-L13) — Subscribes the system к explicit wakes paired with
/// the given opaque wake id. Other systems call <c>scheduler.WakeSystem(target, wakeId)</c>;
/// the registry adds the subscribing system к the next phase's runqueue.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class WakeOnExplicitAttribute : Attribute
{
    /// <summary>Opaque wake id token (caller-defined namespace).</summary>
    public uint WakeId { get; }

    /// <summary>Create the attribute for the given wake id.</summary>
    public WakeOnExplicitAttribute(uint wakeId)
    {
        WakeId = wakeId;
    }
}
