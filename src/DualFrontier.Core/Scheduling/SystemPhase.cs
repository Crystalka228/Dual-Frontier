using System;
using System.Collections.Generic;
using DualFrontier.Core.ECS;

namespace DualFrontier.Core.Scheduling;

/// <summary>
/// Набор систем, которые могут исполняться параллельно (не имеют общих WRITE).
/// Неизменяемая структура: строится один раз <see cref="DependencyGraph"/>
/// и далее только читается планировщиком.
/// </summary>
internal sealed class SystemPhase
{
    /// <summary>
    /// Системы этой фазы. Порядок произвольный — исполнение параллельное.
    /// </summary>
    public IReadOnlyList<SystemBase> Systems { get; }

    /// <summary>
    /// Создаёт фазу из готового списка систем.
    /// </summary>
    public SystemPhase(IReadOnlyList<SystemBase> systems)
    {
        Systems = systems ?? throw new ArgumentNullException(nameof(systems));
    }
}
