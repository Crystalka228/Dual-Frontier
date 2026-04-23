namespace DualFrontier.AI.Jobs;
using DualFrontier.Contracts.Core;

/// <summary>
/// Джоб каста заклинания: подойти на дистанцию → начать каст →
/// дождаться окончания каст-тайма → публикация события в
/// <c>Magic</c> шину через <c>SpellSystem</c>.
///
/// См. GDD "Магия", "Школы магии".
/// </summary>
public sealed class JobCast : IJob
{
    // Dependency properties required by the interface IJob
    /// <summary>Pawn executing this job.</summary>
    public EntityId PawnId { get; private set; }

    /// <summary>Current execution status.</summary>
    public JobStatus Status { get; private set; }

    /// <inheritdoc />
    public void Start()
    {
        throw new NotImplementedException("TODO: Фаза 4 — JobCast.Start: проверка маны и школы, резервирование цели");
    }

    /// <inheritdoc />
    public JobStatus Tick(float delta)
    {
        throw new NotImplementedException("TODO: Фаза 4 — JobCast.Tick: каст-тайм, прерывание при уроне");
    }

    /// <inheritdoc />
    public void Abort()
    {
        throw new NotImplementedException("TODO: Фаза 4 — JobCast.Abort: прервать каст, вернуть ману");
    }
}
