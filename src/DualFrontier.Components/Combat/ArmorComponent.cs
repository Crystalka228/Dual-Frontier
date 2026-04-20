using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Combat;

/// <summary>
/// Броня entity. Три базовых сопротивления: острый (колюще-режущий),
/// тупой (дробящий), тепловой (огнестрел, огненные заклинания).
/// Магические школы могут добавлять дополнительные резисты отдельным
/// компонентом (`MagicResistComponent`, Фаза 6).
/// </summary>
public sealed class ArmorComponent : IComponent
{
    // TODO: public float SharpResist;
    // TODO: public float BluntResist;
    // TODO: public float HeatResist;
}
