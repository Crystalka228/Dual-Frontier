using System.Collections.Generic;
using DualFrontier.Contracts.Core;
using DualFrontier.Events.Magic;

namespace DualFrontier.Systems.Magic.Internal;

/// <summary>
/// Внутренний реестр активных mana-lease. Доступен только ManaSystem и связанным
/// системам Magic. Не экспортируется наружу сборки — тип помечен <c>internal</c>,
/// а сам файл лежит в подкаталоге <c>Internal/</c>, чей границы соблюдаются
/// соглашениями проекта (см. <c>Magic/Internal/README.md</c>).
///
/// Реестр хранит список открытых аренд, отвечает за выдачу <see cref="LeaseId"/>,
/// списание маны каждым тиком и поиск истекших аренд.
/// </summary>
internal sealed class ManaLeaseRegistry
{
    // TODO: Фаза 5 — private readonly Dictionary<LeaseId, ManaLease> _active = new();
    // TODO: Фаза 5 — учёт аренд по кастеру (для ActiveCountForCaster).

    /// <summary>
    /// Открывает новую аренду маны для кастера. Реальная реализация должна
    /// проверить инварианты (<paramref name="drainPerTick"/> &gt; 0,
    /// <paramref name="min"/> не больше <paramref name="max"/>), выдать новый
    /// <see cref="LeaseId"/> через <see cref="LeaseId.New"/>, записать запись
    /// <see cref="ManaLease"/> во внутреннюю коллекцию и вернуть идентификатор.
    /// </summary>
    /// <param name="caster">Маг-кастер, у которого будет списываться мана.</param>
    /// <param name="drainPerTick">Расход маны за тик.</param>
    /// <param name="min">Минимальная длительность аренды в тиках.</param>
    /// <param name="max">Максимальная длительность аренды в тиках.</param>
    public LeaseId Open(EntityId caster, float drainPerTick, int min, int max)
    {
        throw new NotImplementedException("TODO: Фаза 5 — регистрация новой mana-lease и выдача LeaseId");
    }

    /// <summary>
    /// Закрывает аренду по идентификатору. Удаляет запись из внутренней
    /// коллекции и возвращает суммарное списанное количество маны
    /// (<see cref="ManaLease.TotalDrained"/>), которое <c>ManaSystem</c>
    /// использует при публикации <c>ManaLeaseClosed</c>.
    /// </summary>
    /// <param name="id">Идентификатор закрываемой аренды.</param>
    /// <param name="reason">Причина закрытия — прокидывается наружу в
    /// соответствующее событие.</param>
    public float Close(LeaseId id, CloseReason reason)
    {
        throw new NotImplementedException("TODO: Фаза 5 — удаление lease и возврат TotalDrained");
    }

    /// <summary>
    /// Списывает <c>DrainPerTick</c> со всех активных аренд за один тик.
    /// Возвращает список идентификаторов аренд, которые по итогам тика
    /// истекли (достигли <c>MaxDurationTicks</c> или у кастера закончилась
    /// мана) — <c>ManaSystem</c> публикует для них <c>ManaLeaseClosed</c>.
    /// </summary>
    public IReadOnlyList<LeaseId> DrainTick()
    {
        throw new NotImplementedException("TODO: Фаза 5 — проход по всем активным lease, списание маны, выявление истекших");
    }

    /// <summary>
    /// Пытается получить запись об аренде по идентификатору. Возвращает
    /// <c>true</c>, если аренда с таким <paramref name="id"/> открыта, иначе
    /// <c>false</c> и <paramref name="lease"/> = <c>null</c>.
    /// </summary>
    /// <param name="id">Идентификатор искомой аренды.</param>
    /// <param name="lease">Запись об аренде, если найдена.</param>
    public bool TryGet(LeaseId id, out ManaLease? lease)
    {
        throw new NotImplementedException("TODO: Фаза 5 — поиск lease во внутренней коллекции");
    }

    /// <summary>
    /// Возвращает количество активных аренд у данного кастера. Используется
    /// <c>ManaSystem</c> для проверки лимита (<c>RefusalReason.LeaseCapExceeded</c>).
    /// </summary>
    /// <param name="caster">Маг-кастер.</param>
    public int ActiveCountForCaster(EntityId caster)
    {
        throw new NotImplementedException("TODO: Фаза 5 — подсчёт открытых lease на кастера");
    }
}
