using DualFrontier.Contracts.Core;

namespace DualFrontier.Events.Combat;

/// <summary>
/// Подтверждение составного выстрела: и патрон, и мана списаны успешно.
/// Публикуется <c>CombatSystem</c> после того, как обе шины
/// (Inventory и Magic) ответили позитивно на <see cref="CompoundShotIntent"/>
/// с тем же <paramref name="Id"/>.
/// </summary>
/// <param name="Id">Идентификатор транзакции (совпадает с исходным Intent).</param>
/// <param name="Shooter">Стрелок.</param>
/// <param name="Target">Цель.</param>
/// <param name="AmmoType">Тип израсходованного патрона.</param>
/// <param name="ManaCost">Списанная мана.</param>
public sealed record ShootGranted(
    TransactionId Id,
    EntityId Shooter,
    EntityId Target,
    string AmmoType,
    float ManaCost) : IEvent;
