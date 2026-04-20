using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Components.Magic;
using DualFrontier.Core.ECS;
using DualFrontier.Events.Magic;
using DualFrontier.Systems.Magic.Internal;

namespace DualFrontier.Systems.Magic;

/// <summary>
/// Регенерация и расход маны. Обрабатывает <c>ManaIntent</c>
/// (дискретное списание по двухшаговой модели) и
/// <see cref="ManaLeaseOpenRequest"/> (непрерывная аренда по §12.2):
/// открывает аренды через <see cref="ManaLeaseRegistry"/>, каждый тик
/// списывает дренаж у активных аренд и публикует
/// <see cref="ManaLeaseClosed"/> по истечении срока или ресурса.
///
/// Фаза: 1 (параллельно с CombatSystem, WeatherSystem).
/// Тик: NORMAL (15 фреймов).
/// </summary>
[SystemAccess(
    reads:  new[] { typeof(ManaLeaseOpenRequest) },
    writes: new[] { typeof(ManaComponent) },
    bus:    nameof(IGameServices.Magic)
)]
[TickRate(TickRates.NORMAL)]
public sealed class ManaSystem : SystemBase
{
    // TODO: Фаза 5 — заменить на поле, инжектируемое через DI-контейнер,
    // чтобы другие системы Magic могли получить ссылку на тот же реестр.
    private readonly ManaLeaseRegistry _registry = new();

    /// <summary>
    /// TODO: Подписаться на ManaIntent (списание), ManaLeaseOpenRequest
    /// (открытие непрерывной аренды), внутренний CloseRequest.
    /// </summary>
    protected override void Subscribe()
    {
        throw new NotImplementedException("TODO: Фаза 1 — подписка на запросы маны");
    }

    public override void Update(float delta)
    {
        // TODO: Фаза 1 — регенерация маны всем пешкам по их школе и статам.
        // TODO: Фаза 5 — вызов DrainActiveLeases() для списания активных аренд.
    }

    /// <summary>
    /// Обработчик дискретного списания маны (шаг 1 двухшаговой модели).
    /// Публикует <c>ManaGranted</c> если маны достаточно, иначе
    /// <c>ManaRefused</c>.
    /// </summary>
    /// <param name="intent">Намерение потратить ману.</param>
    public void OnManaIntent(ManaIntent intent)
    {
        throw new NotImplementedException("TODO: Фаза 1 — проверка и списание маны по ManaIntent");
    }

    /// <summary>
    /// Обработчик собственного ответа <see cref="ManaGranted"/> (когда ManaSystem
    /// использует двухшаговую модель как клиент, например для поддержки голема).
    /// </summary>
    /// <param name="evt">Подтверждение списания маны.</param>
    public void OnManaGranted(ManaGranted evt)
    {
        throw new NotImplementedException("TODO: Фаза 1 — реакция на ManaGranted");
    }

    /// <summary>
    /// Проверяет <c>Mana.Current</c> (<c>N-1</c>-снимок — см. §12.3), при
    /// достаточном ресурсе открывает lease через
    /// <see cref="ManaLeaseRegistry.Open"/> и публикует
    /// <see cref="ManaLeaseOpened"/>; иначе — <see cref="ManaLeaseRefused"/>
    /// с соответствующей <see cref="RefusalReason"/>.
    /// </summary>
    /// <param name="req">Запрос на открытие непрерывной аренды маны.</param>
    public void OnManaLeaseOpenRequest(ManaLeaseOpenRequest req)
    {
        throw new NotImplementedException("TODO: Фаза 5 — открытие mana-lease и публикация ManaLeaseOpened/Refused");
    }

    /// <summary>
    /// Вызывается на каждом тике <see cref="Update"/>: делегирует
    /// <see cref="ManaLeaseRegistry.DrainTick"/>, получает список истекших
    /// аренд и публикует <see cref="ManaLeaseClosed"/> для каждой.
    /// </summary>
    public void DrainActiveLeases()
    {
        throw new NotImplementedException("TODO: Фаза 5 — тиковое списание активных аренд и публикация истекших");
    }

    /// <summary>
    /// Обработчик явного закрытия аренды (например, заклинание прервано
    /// извне или голем деактивирован). Закрывает запись через
    /// <see cref="ManaLeaseRegistry.Close"/> и публикует
    /// <see cref="ManaLeaseClosed"/> с переданной причиной.
    /// </summary>
    /// <param name="id">Идентификатор закрываемой аренды.</param>
    /// <param name="reason">Причина закрытия.</param>
    public void OnManaLeaseCloseRequest(LeaseId id, CloseReason reason)
    {
        throw new NotImplementedException("TODO: Фаза 5 — явное закрытие mana-lease по запросу");
    }
}
