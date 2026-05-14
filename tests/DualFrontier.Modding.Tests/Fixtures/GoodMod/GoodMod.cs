using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;
using DualFrontier.Core.ECS;

namespace DualFrontier.Modding.Tests.Fixtures.GoodMod;

/// <summary>
/// Minimal legal mod used by pipeline tests. Registers a single system that
/// writes to <see cref="GoodComponent"/> — a component not touched by any
/// core system, so no conflicts arise.
/// </summary>
public sealed class GoodMod : IMod
{
    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        api.RegisterComponent<GoodComponent>();
        api.RegisterSystem<GoodSystem>();
    }

    /// <inheritdoc />
    public void Unload()
    {
        // The test mod holds no resources — nothing to release.
    }
}

/// <summary>
/// Component exclusive to the good mod; no core system references it.
/// Unmanaged struct shape per K-L3 default (RegisterComponent&lt;T&gt; constraint
/// since K8.3+K8.4 IModApi v3). Mods can register class shapes via
/// RegisterManagedComponent&lt;T&gt; with [ManagedStorage] attribute.
/// </summary>
public struct GoodComponent : IComponent
{
    /// <summary>Integer payload; present only so the struct has meaningful size in tests.</summary>
    public int Value;
}

/// <summary>System that writes to <see cref="GoodComponent"/> every tick.</summary>
[SystemAccess(reads: new[] { typeof(GoodComponent) }, writes: new[] { typeof(GoodComponent) }, bus: nameof(IGameServices.World))]
[TickRate(TickRates.NORMAL)]
public sealed class GoodSystem : SystemBase
{
    /// <inheritdoc />
    public override void Update(float delta)
    {
        // Test no-op: the system only needs to exist in the graph.
    }
}
