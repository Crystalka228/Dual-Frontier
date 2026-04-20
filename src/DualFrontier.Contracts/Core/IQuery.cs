using System;

namespace DualFrontier.Contracts.Core;

/// <summary>
/// Маркер-интерфейс синхронного запроса к шине.
/// Запрос — это "вопрос" с ожидаемым <see cref="IQueryResult"/>.
/// В двухшаговой модели запросы предпочтительно заменять на
/// пары Intent + Granted/Refused (см. <c>/docs/EVENT_BUS.md</c>).
/// </summary>
public interface IQuery
{
}
