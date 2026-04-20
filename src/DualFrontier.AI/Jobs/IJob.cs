namespace DualFrontier.AI.Jobs;

/// <summary>
/// Жизненный цикл конкретного джоба.
/// </summary>
public enum JobStatus
{
    /// <summary>Джоб создан, но <see cref="IJob.Start"/> ещё не вызван.</summary>
    NotStarted,

    /// <summary>Джоб исполняется — вернёт следующий шаг на следующем тике.</summary>
    Running,

    /// <summary>Джоб успешно завершился.</summary>
    Done,

    /// <summary>Джоб не смог завершиться (цель пропала, ресурсов нет).</summary>
    Failed
}

/// <summary>
/// Задача, которую выполняет пешка: хаул, крафт, каст,
/// медитация, приказ голему и т. д. JobSystem тикает джоб до
/// завершения, джоб сам не пишет компоненты.
///
/// Синхронный интерфейс — никакого async (см. THREADING и
/// правила <c>DualFrontier.Systems</c>).
/// </summary>
public interface IJob
{
    /// <summary>
    /// Однократная инициализация: взять цель, подписать блокировки.
    /// </summary>
    void Start();

    /// <summary>
    /// Один шаг джоба. Возвращает <see cref="JobStatus"/>.
    /// </summary>
    /// <param name="delta">Прошло реального времени с прошлого тика.</param>
    JobStatus Tick(float delta);

    /// <summary>
    /// Принудительное прерывание: корректно откатить частично
    /// выполненные действия (освободить предметы, снять блокировки).
    /// </summary>
    void Abort();
}
