using DualFrontier.Contracts.Modding;

namespace DualFrontier.Mod.Example;

/// <summary>
/// Reference example mod. Demonstrates the correct pattern:
/// imports come only from DualFrontier.Contracts.* — no references to Core/Systems/Components.
/// </summary>
public sealed class ExampleMod : IMod
{
    /// <summary>TODO: Phase 2 — register components/systems via IModApi.</summary>
    public void Initialize(IModApi api)
    {
        // TODO: api.RegisterComponent<MyCustomComponent>();
        // TODO: api.Subscribe<PawnSpawnedEvent>(OnPawnSpawned);
    }

    /// <summary>TODO: Phase 2 — unsubscribe and release resources on unload.</summary>
    public void Unload()
    {
        // TODO: unsubscribe from events.
    }
}
