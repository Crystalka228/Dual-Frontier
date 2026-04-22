using System;

namespace DualFrontier.Application.Modding;

/// <summary>
/// Бросается, когда мод попытался нарушить изоляцию — например,
/// скастить <see cref="DualFrontier.Contracts.Modding.IModApi"/> к конкретной
/// реализации <see cref="RestrictedModApi"/> или обратиться к внутренностям
/// ядра минуя API.
///
/// Также это исключение бросается из <c>SystemExecutionContext</c>, когда
/// мод-система нарушает изоляцию: обращается к незадекларированному
/// компоненту, публикует событие в чужую шину, лезет напрямую к
/// <c>World</c>/<c>ComponentStore</c> или пытается получить ссылку на
/// другую систему через <c>GetSystem</c>.
///
/// По правилам TechArch 11.8 такой мод немедленно выгружается
/// <c>ModFaultHandler</c> — ядро при этом не падает, игра продолжает работу.
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
