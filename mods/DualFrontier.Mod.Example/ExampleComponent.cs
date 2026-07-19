using DualFrontier.Contracts.Core;

namespace DualFrontier.Mod.Example;

/// <summary>
/// Reference SDK component — a plain <c>unmanaged</c> struct authored against
/// <c>DualFrontier.Contracts</c> alone (K-L3 Path α). Demonstrates that a mod
/// can define real component data without naming any engine assembly.
/// </summary>
public struct ExampleComponent : IComponent
{
    /// <summary>Number of ticks the example system has advanced this entity.</summary>
    public int Ticks;
}
