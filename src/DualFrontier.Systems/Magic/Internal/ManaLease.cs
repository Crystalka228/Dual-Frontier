using DualFrontier.Contracts.Core;
using DualFrontier.Events.Magic;

namespace DualFrontier.Systems.Magic.Internal;

/// <summary>
/// Internal record for an active mana lease. Stores the identifier,
/// caster, drain parameters, and accumulated statistics (ticks and
/// total mana drained). Instantiated exclusively by
/// <see cref="ManaLeaseRegistry"/> and never leaves the
/// <c>DualFrontier.Systems</c> assembly.
/// </summary>
internal sealed class ManaLease
{
    /// <summary>
    /// Lease identifier — matches the identifier published in
    /// <c>ManaLeaseOpened</c>.
    /// </summary>
    public LeaseId Id { get; }

    /// <summary>
    /// The caster mage from whom mana is drained on every tick.
    /// </summary>
    public EntityId Caster { get; }

    /// <summary>
    /// Amount of mana drained from the caster per tick.
    /// </summary>
    public float DrainPerTick { get; }

    /// <summary>
    /// Minimum lease duration in ticks — closing with
    /// <c>CloseReason.Completed</c> is not allowed before this expires.
    /// </summary>
    public int MinDurationTicks { get; }

    /// <summary>
    /// Maximum lease duration in ticks — once reached, the registry
    /// force-closes the lease.
    /// </summary>
    public int MaxDurationTicks { get; }

    /// <summary>
    /// How many ticks have elapsed since the lease was opened.
    /// </summary>
    public int TicksElapsed { get; private set; }

    /// <summary>
    /// Total mana drained over the entire lifetime of the lease.
    /// Used when publishing <c>ManaLeaseClosed.TotalManaDrained</c>.
    /// </summary>
    public float TotalDrained { get; private set; }

    /// <summary>
    /// Creates a lease record. Called only from
    /// <see cref="ManaLeaseRegistry.Open"/>.
    /// </summary>
    /// <param name="id">Lease identifier.</param>
    /// <param name="caster">Caster mage.</param>
    /// <param name="drainPerTick">Mana drain per tick.</param>
    /// <param name="minDurationTicks">Minimum lease duration in ticks.</param>
    /// <param name="maxDurationTicks">Maximum lease duration in ticks.</param>
    public ManaLease(LeaseId id, EntityId caster, float drainPerTick, int minDurationTicks, int maxDurationTicks)
    {
        Id = id;
        Caster = caster;
        DrainPerTick = drainPerTick;
        MinDurationTicks = minDurationTicks;
        MaxDurationTicks = maxDurationTicks;
        TicksElapsed = 0;
        TotalDrained = 0f;
    }

    /// <summary>
    /// Advances the lease by one tick: adds <paramref name="actualDrain"/>
    /// to <see cref="TotalDrained"/> and increments <see cref="TicksElapsed"/>.
    /// Returns <c>true</c> if the lease has reached
    /// <see cref="MaxDurationTicks"/> and must be closed.
    /// </summary>
    /// <param name="actualDrain">Mana actually drained this tick (may differ
    /// from <see cref="DrainPerTick"/> when the pool is exhausted).</param>
    public bool AdvanceTick(float actualDrain)
    {
        throw new NotImplementedException("TODO: Phase 5 — drain mana and advance the lease tick");
    }
}
