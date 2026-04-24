using DualFrontier.Contracts.Core;

namespace DualFrontier.Core.Benchmarks;

/// <summary>
/// Benchmark-only blittable component. The production
/// <c>HealthComponent</c> in <c>DualFrontier.Components.Shared</c> is a
/// reference type (class-based <see cref="IComponent"/>) and therefore
/// cannot cross the P/Invoke boundary without GCHandle pinning. We use this
/// struct to compare like for like: the managed store and the native store
/// both operate on contiguous value-type payloads.
/// </summary>
public struct BenchHealthComponent : IComponent
{
    public int Current;
    public int Maximum;

    public BenchHealthComponent(int current, int maximum)
    {
        Current = current;
        Maximum = maximum;
    }
}
