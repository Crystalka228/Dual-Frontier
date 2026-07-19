using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Mod.Example;

/// <summary>
/// Reference SDK event — published by <see cref="ExampleSystem"/> through the
/// per-tick context. Routes to the World domain bus via <c>[EventBus]</c>. The
/// mod declares the matching <c>kernel.publish/subscribe</c> capability in its
/// manifest, so the engine's capability gate admits it.
/// </summary>
[EventBus("World")]
public sealed record ExampleEvent(long Tick) : IEvent;
