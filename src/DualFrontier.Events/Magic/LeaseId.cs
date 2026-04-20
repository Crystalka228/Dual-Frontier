namespace DualFrontier.Events.Magic;

/// <summary>
/// Идентификатор непрерывной аренды маны (mana lease) — используется для
/// связывания событий жизненного цикла аренды: <c>ManaLeaseOpened</c>,
/// <c>ManaLeaseClosed</c>. Монотонно возрастающее значение, уникально в
/// пределах процесса.
/// </summary>
public readonly record struct LeaseId(ulong Value)
{
    /// <summary>
    /// Фабричный метод для выдачи нового монотонно возрастающего идентификатора.
    /// TODO: Фаза 5 — реализовать счётчик (Interlocked.Increment по приватному
    /// полю), пока выбрасывается <see cref="NotImplementedException"/>.
    /// </summary>
    public static LeaseId New() => throw new NotImplementedException("TODO: Фаза 5 — реализовать монотонный счётчик LeaseId.");
}
