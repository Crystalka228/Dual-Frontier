namespace DualFrontier.AI.Jobs;

/// <summary>
/// Джоб медитации мага: долгая неподвижная работа, которая
/// прибавляет опыт школы (SkillSystem / SchoolComponent) и
/// восстанавливает ману выше обычного порога.
///
/// См. GDD "Рост мага".
/// </summary>
public sealed class JobMeditate : IJob
{
    /// <inheritdoc />
    public void Start()
    {
        throw new NotImplementedException("TODO: Фаза 4 — JobMeditate.Start: занять место медитации, установить таймер");
    }

    /// <inheritdoc />
    public JobStatus Tick(float delta)
    {
        throw new NotImplementedException("TODO: Фаза 4 — JobMeditate.Tick: тики опыта и реген маны");
    }

    /// <inheritdoc />
    public void Abort()
    {
        throw new NotImplementedException("TODO: Фаза 4 — JobMeditate.Abort: прервать, сохранить полученный частичный опыт");
    }
}
