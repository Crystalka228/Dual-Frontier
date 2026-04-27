using System;

namespace DualFrontier.Contracts.Core;

/// <summary>
/// Entity marker interface. Used very rarely — across all APIs the primary
/// entity identifier is <see cref="EntityId"/>. The interface is kept for
/// the rare cases where an alternative ECS-core implementation (e.g. in a
/// mod) wants a base type for a reference-style entity wrapper.
/// </summary>
public interface IEntity
{
}
