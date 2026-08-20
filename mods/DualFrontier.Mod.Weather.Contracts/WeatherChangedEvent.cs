using DualFrontier.Contracts.Core;

namespace DualFrontier.Mod.Weather.Contracts;

/// <summary>
/// Published when the world's weather transitions. MOD-AUTHORED and MOD-OWNED: the capability
/// ledger records this type under <c>mod.dualfrontier.weather.contracts</c>, so any OTHER mod
/// that publishes or subscribes to it must declare
/// <c>mod.dualfrontier.weather.contracts.{publish|subscribe}:DualFrontier.Mod.Weather.Contracts.WeatherChangedEvent</c>
/// in its manifest and list this mod in its dependencies.
///
/// <para>
/// No <c>[EventTier]</c>: the default Normal tier is right for a low-frequency world event, and
/// Normal additionally emits the un-prefixed publish/subscribe alias tokens (S-LOCK-4), which is
/// what the sibling mod's manifest declares.
/// </para>
///
/// <para>
/// Vended from a shared mod because <c>ContractValidator</c> Phase E REJECTS a regular mod that
/// exports an <c>IEvent</c> — cross-mod identities must resolve through the one shared ALC. The
/// structure of this mod pair is forced by that law, not chosen for taste.
/// </para>
/// </summary>
public sealed record WeatherChangedEvent : IEvent
{
    /// <summary>The weather now in effect.</summary>
    public required WeatherKind Kind { get; init; }

    /// <summary>The weather that was in effect immediately before this transition.</summary>
    public required WeatherKind PreviousKind { get; init; }

    /// <summary>How severe this weather is, 0..1. <see cref="WeatherKind.Clear"/> carries 0.</summary>
    public required float Intensity { get; init; }
}
