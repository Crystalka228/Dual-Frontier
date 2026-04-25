using System;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Агрегатор всех доменных шин событий. Точка доступа для систем, модов и
/// приложения. Система выбирает "свою" шину через это свойство и работает
/// только с ней — это декларируется через <c>[SystemAccess(bus: ...)]</c>.
///
/// Имена свойств используются в аргументе <c>bus:</c> атрибута
/// <c>SystemAccessAttribute</c> как <c>nameof(IGameServices.Combat)</c>.
/// </summary>
public interface IGameServices
{
    /// <summary>
    /// Шина боя: ShootAttempt, DamageEvent, DeathEvent, StatusApplied.
    /// Пишут: <c>CombatSystem</c>, <c>ProjectileSystem</c>.
    /// Читают: <c>DamageSystem</c>, <c>StatusEffectSystem</c>.
    /// </summary>
    ICombatBus Combat { get; }

    /// <summary>
    /// Шина склада: AmmoRequest/Result, ItemAdded/Removed, CraftRequest.
    /// Пишут: <c>HaulSystem</c>, <c>CraftSystem</c>.
    /// Читают: <c>InventorySystem</c>, <c>JobSystem</c>.
    /// </summary>
    IInventoryBus Inventory { get; }

    /// <summary>
    /// Шина магии: ManaRequest/Result, SpellCast, EtherSurge, GolemActivated.
    /// Пишут: <c>SpellSystem</c>, <c>GolemSystem</c>.
    /// Читают: <c>ManaSystem</c>, <c>EtherGrowthSystem</c>.
    /// </summary>
    IMagicBus Magic { get; }

    /// <summary>
    /// Шина пешек: MoodBreak, DeathReaction, SkillGain.
    /// Пишут: <c>NeedsSystem</c>, <c>MoodSystem</c>.
    /// Читают: <c>JobSystem</c>, <c>SocialSystem</c>.
    /// </summary>
    IPawnBus Pawns { get; }

    /// <summary>
    /// Шина мира: EtherNodeChanged, WeatherChanged, RaidIncoming.
    /// Пишут: <c>BiomeSystem</c>, <c>WeatherSystem</c>.
    /// Читают: <c>EtherGridSystem</c>, <c>RaidSystem</c>.
    /// </summary>
    IWorldBus World { get; }

    /// <summary>
    /// Шина промышленной энергосети: PowerRequest, PowerGranted, GridOverload,
    /// ConverterPowerOutput. Пишут: <c>ElectricGridSystem</c>,
    /// <c>ConverterSystem</c>. Читают: <c>ElectricGridSystem</c>, потребители,
    /// UI. Введена TechArch v0.3 §13.1.
    /// </summary>
    IPowerBus Power { get; }
}
