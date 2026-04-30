using System;
using System.Collections.Generic;
using DualFrontier.Contracts.Modding;
using Fixture.SharedEvents;

namespace Fixture.SubscriberMod;

/// <summary>
/// Test fixture mod that subscribes to <see cref="SharedTestEvent"/>
/// during <see cref="Initialize"/> and stores received payloads on the
/// instance. The test driver retains the <see cref="IMod"/> instance and
/// reads <see cref="ReceivedPayloads"/> via reflection because the
/// concrete type lives inside the mod's own AssemblyLoadContext.
/// </summary>
public sealed class Subscriber : IMod
{
    private readonly List<string> _received = new();

    /// <summary>
    /// Payloads of every <see cref="SharedTestEvent"/> the subscriber
    /// has received in the order they arrived.
    /// </summary>
    public IReadOnlyList<string> ReceivedPayloads => _received;

    /// <summary>
    /// The shared event type as observed from inside this mod's
    /// AssemblyLoadContext. Used by tests to verify cross-ALC type
    /// identity with the shared ALC's instance of the same type.
    /// </summary>
    public Type SharedEventType => typeof(SharedTestEvent);

    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        if (api is null) throw new ArgumentNullException(nameof(api));
        api.Subscribe<SharedTestEvent>(OnSharedEvent);
    }

    /// <inheritdoc />
    public void Unload()
    {
    }

    private void OnSharedEvent(SharedTestEvent evt) => _received.Add(evt.Payload);
}
