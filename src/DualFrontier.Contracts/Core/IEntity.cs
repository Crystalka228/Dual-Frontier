using System;

namespace DualFrontier.Contracts.Core;

/// <summary>
/// Маркер-интерфейс сущности. Используется крайне редко — основным
/// идентификатором сущности во всех API служит <see cref="EntityId"/>.
/// Интерфейс оставлен для тех редких случаев, когда реализация ECS ядра
/// (например, в моде) хочет иметь опорный тип для референсной entity-обёртки.
/// </summary>
public interface IEntity
{
}
