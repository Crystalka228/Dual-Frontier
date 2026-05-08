using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Combat;

/// <summary>
/// Entity armour. Three basic resistances: sharp (piercing/cutting),
/// blunt (bludgeoning), and heat (firearms, fire spells).
/// Magic schools may add extra resistances via a separate component
/// (`MagicResistComponent`, Phase 6).
/// </summary>
[ModAccessible(Read = true)]
public struct ArmorComponent : IComponent
{
    public float SharpResist;
    public float BluntResist;
    public float HeatResist;
}
