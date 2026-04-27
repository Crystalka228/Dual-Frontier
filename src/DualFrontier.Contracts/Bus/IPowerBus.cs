using System;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Power-domain bus. Industrial electric-grid events: power request,
/// granted watts, grid overload, converter power output.
/// Writers: <c>ElectricGridSystem</c>, <c>ConverterSystem</c>.
/// Readers: <c>ElectricGridSystem</c>, consumers, UI.
///
/// Introduced in TechArch v0.3 §13.1: originally ElectricGrid+Converter
/// mistakenly published on <see cref="IInventoryBus"/>; moved here for
/// alignment with the GDD's "dual industrial/magic branches".
/// EtherGrid (the magic energy network) stays on <see cref="IWorldBus"/>
/// until Phase 6.
/// </summary>
public interface IPowerBus : IEventBus
{
}
