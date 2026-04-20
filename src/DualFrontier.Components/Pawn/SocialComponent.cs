using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Pawn;

/// <summary>
/// Социальные отношения пешки с другими. Ключ — EntityId другой пешки,
/// значение — числовой уровень симпатии (отрицательные = вражда).
/// Обновляется SocialSystem (RARE tick) при взаимодействиях.
/// </summary>
public sealed class SocialComponent : IComponent
{
    // TODO: public Dictionary<EntityId, int> Relations = new();
}
