namespace DualFrontier.AI.Jobs;

/// <summary>
/// Джоб работы на верстаке: дойти → проверить ингредиенты →
/// стоять и тикать прогресс → при готовности отдать продукт
/// через <c>CraftSystem</c>.
///
/// См. GDD "Ресурсы и производство".
/// </summary>
public sealed class JobCraft : IJob
{
    /// <inheritdoc />
    public void Start()
    {
        throw new NotImplementedException("TODO: Фаза 6 — JobCraft.Start: зарезервировать верстак и рецепт");
    }

    /// <inheritdoc />
    public JobStatus Tick(float delta)
    {
        throw new NotImplementedException("TODO: Фаза 6 — JobCraft.Tick: продвижение прогресса с учётом навыка");
    }

    /// <inheritdoc />
    public void Abort()
    {
        throw new NotImplementedException("TODO: Фаза 6 — JobCraft.Abort: отпустить верстак, вернуть ингредиенты");
    }
}
