using DualFrontier.Contracts.Modding;

namespace DualFrontier.Mod.Example;

/// <summary>
/// Reference example mod. Demonstrates the correct pattern: imports come only
/// from DualFrontier.Contracts.* — no references to Core/Systems/Components. It
/// registers a real component and a real <c>ISimulationSystem</c> that reads,
/// writes, and publishes through the W1 SDK surface.
/// </summary>
public sealed class ExampleMod : IMod
{
    /// <summary>Registers the example component and system with the engine.</summary>
    public void Initialize(IModApi api)
    {
        api.RegisterComponent<ExampleComponent>();
        api.RegisterSystem<ExampleSystem>();
    }

    /// <summary>
    /// Unload hook. The system's event subscription is released by the engine's
    /// mod-unload chain (RestrictedModApi.UnsubscribeAll), so there is nothing to
    /// release here.
    /// </summary>
    public void Unload()
    {
    }
}
