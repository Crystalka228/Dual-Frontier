using System;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Aggregator of all domain event buses. Access point for systems, mods, and
/// the application layer. A system selects "its" bus through this property
/// and works only with that bus — declared via <c>[SystemAccess(bus: ...)]</c>.
///
/// The property names are used in the <c>bus:</c> argument of
/// <c>SystemAccessAttribute</c> as <c>nameof(IGameServices.Combat)</c>.
/// </summary>
public interface IGameServices
{
    /// <summary>
    /// Combat bus: ShootAttempt, DamageEvent, DeathEvent, StatusApplied.
    /// Writers: <c>CombatSystem</c>, <c>ProjectileSystem</c>.
    /// Readers: <c>DamageSystem</c>, <c>StatusEffectSystem</c>.
    /// </summary>
    ICombatBus Combat { get; }

    /// <summary>
    /// Inventory bus: AmmoRequest/Result, ItemAdded/Removed, CraftRequest.
    /// Writers: <c>HaulSystem</c>, <c>CraftSystem</c>.
    /// Readers: <c>InventorySystem</c>, <c>JobSystem</c>.
    /// </summary>
    IInventoryBus Inventory { get; }

    /// <summary>
    /// Magic bus: ManaRequest/Result, SpellCast, EtherSurge, GolemActivated.
    /// Writers: <c>SpellSystem</c>, <c>GolemSystem</c>.
    /// Readers: <c>ManaSystem</c>, <c>EtherGrowthSystem</c>.
    /// </summary>
    IMagicBus Magic { get; }

    /// <summary>
    /// Pawn bus: MoodBreak, DeathReaction, SkillGain.
    /// Writers: <c>NeedsSystem</c>, <c>MoodSystem</c>.
    /// Readers: <c>JobSystem</c>, <c>SocialSystem</c>.
    /// </summary>
    IPawnBus Pawns { get; }

    /// <summary>
    /// World bus: EtherNodeChanged, WeatherChanged, RaidIncoming.
    /// Writers: <c>BiomeSystem</c>, <c>WeatherSystem</c>.
    /// Readers: <c>EtherGridSystem</c>, <c>RaidSystem</c>.
    /// </summary>
    IWorldBus World { get; }

    /// <summary>
    /// Industrial power-grid bus: PowerRequest, PowerGranted, GridOverload,
    /// ConverterPowerOutput. Writers: <c>ElectricGridSystem</c>,
    /// <c>ConverterSystem</c>. Readers: <c>ElectricGridSystem</c>, consumers,
    /// UI. Introduced in TechArch v0.3 §13.1.
    /// </summary>
    IPowerBus Power { get; }
}
