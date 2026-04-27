using System;

namespace DualFrontier.Contracts.Core;

/// <summary>
/// Marker interface for a synchronous bus query.
/// A query is a "question" with an expected <see cref="IQueryResult"/>.
/// In the two-step model, queries are preferably replaced with
/// Intent + Granted/Refused pairs (see <c>/docs/EVENT_BUS.md</c>).
/// </summary>
public interface IQuery
{
}
