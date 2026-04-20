using System;

namespace DualFrontier.Core.ECS;

/// <summary>
/// Исключение, бросаемое <c>SystemExecutionContext</c> при нарушении изоляции:
/// обращение к незадекларированному компоненту, прямой доступ к другой системе,
/// использование World в обход сторожа. Сообщение содержит имя нарушающей
/// системы, имя нарушенного типа и подсказку, как исправить декларацию.
/// </summary>
public sealed class IsolationViolationException : Exception
{
    /// <summary>
    /// Создаёт исключение с диагностическим сообщением. Формат сообщения
    /// фиксирован и используется тестами — не менять без согласования.
    /// </summary>
    public IsolationViolationException(string message) : base(message)
    {
    }
}
