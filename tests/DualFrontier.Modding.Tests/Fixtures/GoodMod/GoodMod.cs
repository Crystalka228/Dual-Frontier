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
        // Тестовый мод не удерживает ресурсов — очищать нечего.
    }
}

/// <summary>Component exclusive to the good mod; no core system references it.</summary>
public sealed class GoodComponent : IComponent
{
    /// <summary>Integer payload; present only so the struct has meaningful size in tests.</summary>
    public int Value { get; init; }
}

/// <summary>System that writes to <see cref="GoodComponent"/> every tick.</summary>
[SystemAccess(reads: new[] { typeof(GoodComponent) }, writes: new[] { typeof(GoodComponent) }, bus: nameof(IGameServices.World))]
[TickRate(TickRates.NORMAL)]
public sealed class GoodSystem : SystemBase
{
    /// <inheritdoc />
    public override void Update(float delta)
    {
        // Тестовый no-op: системе достаточно существовать в графе.
    }
}
