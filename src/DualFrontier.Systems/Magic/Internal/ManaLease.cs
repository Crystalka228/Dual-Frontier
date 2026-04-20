using DualFrontier.Contracts.Core;
using DualFrontier.Events.Magic;

namespace DualFrontier.Systems.Magic.Internal;

/// <summary>
/// Внутренняя запись об активной аренде маны. Хранит идентификатор,
/// кастера, параметры дренажа и накопленную статистику (тики и суммарный
/// расход маны). Инстанцируется исключительно <see cref="ManaLeaseRegistry"/>
/// и не покидает сборки <c>DualFrontier.Systems</c>.
/// </summary>
internal sealed class ManaLease
{
    /// <summary>
    /// Идентификатор аренды — совпадает с идентификатором, опубликованным
    /// в <c>ManaLeaseOpened</c>.
    /// </summary>
    public LeaseId Id { get; }

    /// <summary>
    /// Маг-кастер, у которого списывается мана на каждом тике.
    /// </summary>
    public EntityId Caster { get; }

    /// <summary>
    /// Количество маны, которое списывается у кастера за один тик.
    /// </summary>
    public float DrainPerTick { get; }

    /// <summary>
    /// Минимальная длительность аренды в тиках — до её истечения закрытие
    /// <c>CloseReason.Completed</c> не допускается.
    /// </summary>
    public int MinDurationTicks { get; }

    /// <summary>
    /// Максимальная длительность аренды в тиках — по достижении реестр
    /// принудительно закрывает аренду.
    /// </summary>
    public int MaxDurationTicks { get; }

    /// <summary>
    /// Сколько тиков уже прошло с момента открытия аренды.
    /// </summary>
    public int TicksElapsed { get; private set; }

    /// <summary>
    /// Суммарное количество маны, списанное за всё время жизни аренды.
    /// Используется при публикации <c>ManaLeaseClosed.TotalManaDrained</c>.
    /// </summary>
    public float TotalDrained { get; private set; }

    /// <summary>
    /// Создаёт запись об аренде. Вызывается только из
    /// <see cref="ManaLeaseRegistry.Open"/>.
    /// </summary>
    /// <param name="id">Идентификатор аренды.</param>
    /// <param name="caster">Маг-кастер.</param>
    /// <param name="drainPerTick">Расход маны за тик.</param>
    /// <param name="minDurationTicks">Минимальная длительность аренды в тиках.</param>
    /// <param name="maxDurationTicks">Максимальная длительность аренды в тиках.</param>
    public ManaLease(LeaseId id, EntityId caster, float drainPerTick, int minDurationTicks, int maxDurationTicks)
    {
        Id = id;
        Caster = caster;
        DrainPerTick = drainPerTick;
        MinDurationTicks = minDurationTicks;
        MaxDurationTicks = maxDurationTicks;
        TicksElapsed = 0;
        TotalDrained = 0f;
    }

    /// <summary>
    /// Продвигает аренду на один тик: списывает <paramref name="actualDrain"/>
    /// в <see cref="TotalDrained"/> и инкрементирует <see cref="TicksElapsed"/>.
    /// Возвращает <c>true</c>, если аренда достигла
    /// <see cref="MaxDurationTicks"/> и должна быть закрыта.
    /// </summary>
    /// <param name="actualDrain">Фактически списанное количество маны за тик
    /// (может отличаться от <see cref="DrainPerTick"/> при исчерпании пула).</param>
    public bool AdvanceTick(float actualDrain)
    {
        throw new NotImplementedException("TODO: Фаза 5 — списание маны и продвижение тика аренды");
    }
}
