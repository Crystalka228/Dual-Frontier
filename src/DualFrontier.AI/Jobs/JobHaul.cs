namespace DualFrontier.AI.Jobs;

/// <summary>
/// Джоб переноски стака: дойти до источника → взять → дойти до
/// цели → положить. Работает вместе с <c>HaulSystem</c> и
/// <c>InventorySystem</c> через intent-события.
///
/// См. GDD разделы "Ресурсы и производство", "Пешки".
/// </summary>
public sealed class JobHaul : IJob
{
    /// <inheritdoc />
    public void Start()
    {
        throw new NotImplementedException("TODO: Фаза 3 — JobHaul.Start: взять цели источника и назначения");
    }

    /// <inheritdoc />
    public JobStatus Tick(float delta)
    {
        throw new NotImplementedException("TODO: Фаза 3 — JobHaul.Tick: шаги GoTo/Pickup/Carry/Drop");
    }

    /// <inheritdoc />
    public void Abort()
    {
        throw new NotImplementedException("TODO: Фаза 3 — JobHaul.Abort: бросить стак, снять блокировку источника");
    }
}
