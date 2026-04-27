using System;

namespace DualFrontier.Contracts.Core;

/// <summary>
/// Marker interface for an ECS component.
/// A component is a POCO: data only, no logic.
/// Logic lives in systems. See <c>/docs/ECS.md</c> for the rules.
/// </summary>
public interface IComponent
{
}
