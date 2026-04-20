using System;

namespace DualFrontier.Contracts.Core;

/// <summary>
/// Маркер-интерфейс компонента ECS.
/// Компонент — это POCO: только данные, никакой логики.
/// Логика живёт в системах. См. <c>/docs/ECS.md</c> для правил.
/// </summary>
public interface IComponent
{
}
