namespace DualFrontier.Events.Combat;

/// <summary>
/// Идентификатор составной транзакции выстрела (двухфазный коммит между
/// <c>InventoryBus</c> и <c>MagicBus</c>). Используется для связывания
/// <c>CompoundShotIntent</c> с ответами <c>ShootGranted</c> / <c>ShootRefused</c>.
/// Монотонно возрастающее значение, уникально в пределах процесса.
/// </summary>
public readonly record struct TransactionId(ulong Value)
{
    /// <summary>
    /// Фабричный метод для выдачи нового монотонно возрастающего идентификатора.
    /// TODO: Фаза 4 — реализовать счётчик (Interlocked.Increment по приватному
    /// полю), пока выбрасывается <see cref="NotImplementedException"/>.
    /// </summary>
    public static TransactionId New() => throw new NotImplementedException("TODO: Фаза 4 — реализовать монотонный счётчик TransactionId.");
}
