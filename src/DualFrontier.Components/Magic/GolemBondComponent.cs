using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Enums;

namespace DualFrontier.Components.Magic;

/// <summary>
/// Связь голема с магом-хозяином. Без хозяина голем не функционирует
/// (см. GDD 5.2 «Уязвимости»: истощение/смерть мага → остановка големов).
/// <c>GolemTier</c> — уровень голема 1..5 (GDD 5.1 «Типы Големов по Уровням»).
///
/// v02 Addendum (§12.5): добавлены поля режима владения и счётчика оспаривания
/// для реализации механики перехвата/покидания голема через события
/// <c>GolemOwnershipTransferRequest</c> / <c>GolemOwnershipChanged</c>.
/// </summary>
public sealed class GolemBondComponent : IComponent
{
    // TODO: public EntityId? OwnerId;
    // TODO: public int GolemTier;  // 1..5 — см. GDD 5.1

    /// <summary>
    /// Текущий маг-хозяин. <c>null</c>, если голем в режиме
    /// <see cref="OwnershipMode.Abandoned"/>. TODO: Фаза 6 — связать с
    /// жизненным циклом мага (при смерти хозяина переводить в Abandoned).
    /// </summary>
    public EntityId? BondedMage { get; init; }

    /// <summary>
    /// Режим владения големом. По умолчанию <see cref="OwnershipMode.Bonded"/>.
    /// TODO: Фаза 6 — машина состояний переходов через
    /// <c>GolemOwnershipTransferRequest</c>.
    /// </summary>
    public OwnershipMode Mode { get; init; } = OwnershipMode.Bonded;

    /// <summary>
    /// Счётчик тиков, прошедших с момента начала оспаривания владения.
    /// Используется <c>GolemBondSystem</c> для таймаута смены хозяина в
    /// режиме <see cref="OwnershipMode.Contested"/>.
    /// TODO: Фаза 6 — определить пороговое значение таймаута.
    /// </summary>
    public int TicksSinceContested { get; init; }

    /// <summary>
    /// Прочность связи с хозяином: чем выше, тем сложнее перехватить
    /// управление големом. Участвует в разрешении споров (contest resolution).
    /// TODO: Фаза 6 — формула зависимости от уровня мага/школы.
    /// </summary>
    public int BondStrength { get; init; }
}
