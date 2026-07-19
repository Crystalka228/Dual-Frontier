using DualFrontier.Contracts.Core;

namespace Fixture.SharedEvents;

/// <summary>
/// Event record vended by the shared test fixture. After the W2/BD-3 taxonomy collapse it
/// routes through the single managed dispatch like every other event -- no <c>[EventBus]</c>
/// genre marker, no <c>ModBusRouter</c>. Default delivery (synchronous) is used so the
/// cross-ALC pub/sub test sees the handler fire inside the publisher's call stack.
/// </summary>
public sealed record SharedTestEvent(string Payload) : IEvent;
