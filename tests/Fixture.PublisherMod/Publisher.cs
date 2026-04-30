using System;
using DualFrontier.Contracts.Modding;
using Fixture.SharedEvents;

namespace Fixture.PublisherMod;

/// <summary>
/// Test fixture mod that publishes <see cref="SharedTestEvent"/> values
/// through its <see cref="IModApi"/>. The test driver retains the
/// <see cref="IMod"/> instance and invokes <see cref="TriggerPublish"/>
/// via reflection because the concrete type lives inside the mod's own
/// AssemblyLoadContext and is not visible to the test ALC.
/// </summary>
public sealed class Publisher : IMod
{
    private IModApi? _api;

    /// <summary>
    /// The shared event type as observed from inside this mod's
    /// AssemblyLoadContext. Used by tests to verify cross-ALC type
    /// identity with the shared ALC's instance of the same type.
    /// </summary>
    public Type SharedEventType => typeof(SharedTestEvent);

    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));
    }

    /// <inheritdoc />
    public void Unload()
    {
    }

    /// <summary>
    /// Publishes a <see cref="SharedTestEvent"/> with the given payload via
    /// the API captured during <see cref="Initialize"/>. Invoked by the
    /// cross-ALC pub/sub test through reflection.
    /// </summary>
    public void TriggerPublish(string payload)
    {
        if (_api is null)
            throw new InvalidOperationException("Publisher.TriggerPublish called before Initialize.");
        _api.Publish(new SharedTestEvent(payload));
    }
}
