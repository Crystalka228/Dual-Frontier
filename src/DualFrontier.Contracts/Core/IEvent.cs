using System;

namespace DualFrontier.Contracts.Core;

/// <summary>
/// Marker interface for a domain-bus event.
/// Events are immutable records published via <c>IEventBus.Publish</c>.
/// For deferred delivery — mark the event with the <c>[Deferred]</c>
/// attribute. For immediate delivery (interrupts the phase) — <c>[Immediate]</c>.
/// </summary>
public interface IEvent
{
}
