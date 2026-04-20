using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.World;

/// <summary>
/// Эфирный узел — источник усиленной регенерации маны в радиусе <c>Radius</c>.
/// См. GDD 4 «Магическая Колония»: медитация у узла ×2–3 к регену маны
/// (GDD 4.2, 5.3). <c>Tier</c> — уровень узла (1..N), влияет на силу.
/// Изменение свойств публикует <c>EtherNodeChangedEvent</c>.
/// </summary>
public sealed class EtherNodeComponent : IComponent
{
    // TODO: public int Tier;
    // TODO: public float Radius;
}
