using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Отказ в проведении составного выстрела. Публикуется <c>CombatSystem</c>
/// если хотя бы одна из двух шин (Inventory или Magic) отказала на
/// <see cref="CompoundShotIntent"/>. AI/игрок выбирают альтернативное действие.
/// </summary>
/// <param name="Id">Идентификатор транзакции (совпадает с исходным Intent).</param>
/// <param name="Shooter">Стрелок, которому отказано.</param>
/// <param name="Reason">Причина отказа.</param>
public sealed record ShootRefused(
    TransactionId Id,
    EntityId Shooter,
    ShotRefusalReason Reason) : IEvent;
