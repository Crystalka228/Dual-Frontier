using System;

namespace DualFrontier.Contracts.Bus;

/// <summary>
/// Шина энергодомена. События промышленной электросети: запрос мощности,
/// выдача ватт, перегрузка сети, выход энергии конвертора.
/// Пишут: <c>ElectricGridSystem</c>, <c>ConverterSystem</c>.
/// Читают: <c>ElectricGridSystem</c>, потребители, UI.
///
/// Введено TechArch v0.3 §13.1: исходно ElectricGrid+Converter ошибочно
/// публиковали в <see cref="IInventoryBus"/>; перенесены сюда для
/// согласованности с GDD «дуальные индустриальная/магическая ветки».
/// EtherGrid (магическая энергосеть) остаётся на <see cref="IWorldBus"/>
/// до Phase 6.
/// </summary>
public interface IPowerBus : IEventBus
{
}
