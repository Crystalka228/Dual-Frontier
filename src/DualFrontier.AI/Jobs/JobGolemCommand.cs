namespace DualFrontier.AI.Jobs;

/// <summary>
/// Джоб приказа голему: хозяин отдаёт команду (идти сюда,
/// атаковать это), голем исполняет. Работает через
/// <c>GolemSystem</c> и <c>Magic</c> шину.
///
/// См. GDD "Големы".
/// </summary>
public sealed class JobGolemCommand : IJob
{
    /// <inheritdoc />
    public void Start()
    {
        throw new NotImplementedException("TODO: Фаза 4 — JobGolemCommand.Start: проверить связь хозяин-голем");
    }

    /// <inheritdoc />
    public JobStatus Tick(float delta)
    {
        throw new NotImplementedException("TODO: Фаза 4 — JobGolemCommand.Tick: исполнить команду шагами");
    }

    /// <inheritdoc />
    public void Abort()
    {
        throw new NotImplementedException("TODO: Фаза 4 — JobGolemCommand.Abort: отменить команду, вернуть голема в idle");
    }
}
