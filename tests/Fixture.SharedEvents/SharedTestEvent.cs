using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;

namespace Fixture.SharedEvents;

/// <summary>
/// Event record vended by the shared test fixture. Routed to the World bus
/// via <see cref="EventBusAttribute"/> so <c>RestrictedModApi.Publish</c>
/// can deliver it via <c>ModBusRouter</c>. Default delivery (synchronous)
/// is used so the cross-ALC pub/sub test sees the handler fire inside the
/// publisher's call stack.
/// </summary>
[EventBus("World")]
public sealed record SharedTestEvent(string Payload) : IEvent;
