using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Combat;

/// <summary>
/// Entity armour. Three basic resistances: sharp (piercing/cutting),
/// blunt (bludgeoning), and heat (firearms, fire spells).
/// Magic schools may add extra resistances via a separate component
/// (`MagicResistComponent`, Phase 6).
/// </summary>
public sealed class ArmorComponent : IComponent
{
    // TODO: public float SharpResist;
    // TODO: public float BluntResist;
    // TODO: public float HeatResist;
}
