using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Намерение произвести «составной» выстрел (патрон + магический компонент)
/// в рамках двухфазного коммита: <c>CombatSystem</c> сначала опрашивает
/// <c>InventoryBus</c> (есть ли патрон) и <c>MagicBus</c> (хватает ли маны),
/// и только затем публикует итог. Ответ — <see cref="ShootGranted"/> или
/// <see cref="ShootRefused"/> с тем же <paramref name="Id"/>.
/// </summary>
/// <param name="Id">Идентификатор транзакции для связывания запроса и ответа.</param>
/// <param name="Shooter">Стрелок.</param>
/// <param name="Target">Цель.</param>
/// <param name="AmmoType">Строковый идентификатор типа патрона
/// (TODO: Фаза 4 — заменить на <c>AmmoType</c> enum).</param>
/// <param name="ManaCost">Стоимость магической составляющей выстрела в мане.</param>
public sealed record CompoundShotIntent(
    TransactionId Id,
    EntityId Shooter,
    EntityId Target,
    string AmmoType,
    float ManaCost) : IQuery;
