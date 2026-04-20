using System;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Бросается, когда мод попытался нарушить изоляцию — например,
/// скастить <see cref="DualFrontier.Contracts.Modding.IModApi"/> к конкретной
/// реализации <see cref="RestrictedModApi"/> или обратиться к внутренностям
/// ядра минуя API. По правилам TechArch 11.8 такой мод немедленно выгружается.
/// </summary>
public sealed class ModIsolationException : Exception
{
    /// <summary>
    /// Создаёт исключение без сообщения.
    /// </summary>
    public ModIsolationException()
    {
    }

    /// <summary>
    /// Создаёт исключение с диагностическим сообщением.
    /// </summary>
    /// <param name="message">Описание нарушения.</param>
    public ModIsolationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Создаёт исключение-обёртку над внутренним.
    /// </summary>
    /// <param name="message">Описание нарушения.</param>
    /// <param name="innerException">Исходное исключение.</param>
    public ModIsolationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
