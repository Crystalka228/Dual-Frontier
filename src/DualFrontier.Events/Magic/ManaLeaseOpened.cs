using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Magic;

/// <summary>
/// Confirmation that a mana lease has been opened. Published by <c>ManaSystem</c>
/// immediately after the first drain tick has been successfully charged to the caster.
/// Subscribers (VFX/SFX, UI) turn on continuous-effect visualization.
/// </summary>
/// <param name="Id">Lease identifier — used to correlate the
/// subsequent <c>ManaLeaseClosed</c>.</param>
/// <param name="Caster">The mage caster.</param>
/// <param name="DrainPerTick">Actual mana drain per tick.</param>
public sealed record ManaLeaseOpened(
    LeaseId Id,
    EntityId Caster,
    float DrainPerTick) : IEvent;
