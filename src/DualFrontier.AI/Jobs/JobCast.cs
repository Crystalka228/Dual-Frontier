namespace DualFrontier.AI.Jobs;

/// <summary>
/// Джоб каста заклинания: подойти на дистанцию → начать каст →
/// дождаться окончания каст-тайма → публикация события в
/// <c>Magic</c> шину через <c>SpellSystem</c>.
///
/// См. GDD "Магия", "Школы магии".
/// </summary>
public sealed class JobCast : IJob
{
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
