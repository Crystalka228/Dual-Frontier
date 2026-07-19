using DualFrontier.Contracts.Core;
using DualFrontier.Core.Bus;
using AwesomeAssertions;
using Xunit;

namespace DualFrontier.Core.Tests.Bus;

public sealed record ParityProbeEvent(int Value) : IEvent;

/// <summary>
/// W2/BD-3 collapse parity. The five <see cref="IGameServices"/> genre getters are cosmetic
/// bridges over ONE <see cref="DomainEventBus"/>. Delivery is type-keyed, so:
/// (1) the pre-collapse same-getter path is preserved, and (2) an event subscribed through one
/// getter is now delivered when published through any getter -- the taxonomy no longer partitions
/// routing. This is the behavioral evidence for the "one generic router" claim (BD-3b).
/// </summary>
public sealed class UnifiedBusParityTests
{
    [Fact]
    public void SameGetterDelivery_IsPreserved()
    {
        var services = new GameServices();
        ParityProbeEvent? received = null;
        services.Pawns.Subscribe<ParityProbeEvent>(e => received = e);

        services.Pawns.Publish(new ParityProbeEvent(7));

        received.Should().NotBeNull("the pre-collapse same-genre delivery path is unchanged");
        received!.Value.Should().Be(7);
    }

    [Fact]
    public void EventSubscribedViaOneGetter_IsDeliveredViaAnotherGetter()
    {
        var services = new GameServices();
        ParityProbeEvent? received = null;
        services.Combat.Subscribe<ParityProbeEvent>(e => received = e);

        // Published through a DIFFERENT genre getter -- pre-collapse this reached a separate bus
        // and would NOT deliver; post-collapse every getter is the one unified dispatch.
        services.Pawns.Publish(new ParityProbeEvent(42));

        received.Should().NotBeNull("all getters route to the same unified dispatch post-collapse");
        received!.Value.Should().Be(42);
    }
}
