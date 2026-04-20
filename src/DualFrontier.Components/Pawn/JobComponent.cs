using DualFrontier.Contracts.Core;

namespace DualFrontier.Components.Pawn;

/// <summary>
/// Текущая работа пешки. JobSystem назначает и очищает; AI выбирает
/// следующую. <c>Target</c> — цель работы (entity для добычи/постройки и т. п.)
/// или null, если работа не требует цели (например, отдых).
/// </summary>
public sealed class JobComponent : IComponent
{
    // TODO: создать DualFrontier.Components.Pawn.JobKind enum (Idle, Haul, Build, Mine, Research, Fight, Meditate …) — Фаза 2
    // TODO: public JobKind Current;
    // TODO: public EntityId? Target;
}
