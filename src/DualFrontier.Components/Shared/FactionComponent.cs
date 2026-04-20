using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Shared;

/// <summary>
/// Принадлежность entity к фракции. Используется для определения
/// «свой/чужой» в боевых и дипломатических системах.
/// </summary>
public sealed class FactionComponent : IComponent
{
    // TODO: public string FactionId = string.Empty;  // или int ID в таблице фракций — Фаза 2
}
