using DualFrontier.Contracts.Core;

namespace DualFrontier.Mod.Weather;

/// <summary>
/// The world's current weather, held on a single "weather singleton" entity this mod mints and
/// owns. A plain <c>unmanaged</c> struct authored against <c>DualFrontier.Contracts</c> alone
/// (K-L3 Path α).
///
/// <para>
/// Lives in the REGULAR mod, not the shared one: Phase E binds only <c>IEvent</c> and
/// <c>IModContract</c> to shared mods, and this component is private state no other mod reads.
/// It deliberately carries NO <c>[ModAccessible]</c> — cross-mod component access is not part of
/// this wave, so the component emits no read/write capability tokens at all.
/// </para>
///
/// <para>
/// <see cref="Kind"/> is an <c>int</c> rather than the enum because a component must be
/// <c>unmanaged</c> and stored in native memory with a stable layout; the enum is the authoring
/// vocabulary, the int is the storage.
/// </para>
/// </summary>
public struct WeatherStateComponent : IComponent
{
    /// <summary>The active <c>WeatherKind</c>, stored as its integer value.</summary>
    public int Kind;

    /// <summary>Severity of the active weather, 0..1.</summary>
    public float Intensity;

    /// <summary>SimTick at which the active weather began. Drives the transition cadence.</summary>
    public long LastTransitionTick;
}
