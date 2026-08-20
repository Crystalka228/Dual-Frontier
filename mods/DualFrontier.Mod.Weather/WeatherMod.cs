using DualFrontier.Contracts.Modding;

namespace DualFrontier.Mod.Weather;

/// <summary>
/// Entry point for the weather mechanic. Registers one component and two systems through
/// <see cref="IModApi"/> and nothing else -- all behaviour lives in the systems.
///
/// <para>
/// Imports come only from <c>DualFrontier.Contracts.*</c> and from this mod own shared contracts
/// mod. No engine assembly is named anywhere in this project.
/// </para>
/// </summary>
public sealed class WeatherMod : IMod
{
    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        api.RegisterComponent<WeatherStateComponent>();
        api.RegisterSystem<WeatherSystem>();
        api.RegisterSystem<WeatherPresentationSystem>();
    }

    /// <summary>
    /// Unload hook. The engine unload chain releases the presentation system subscription
    /// (<c>RestrictedModApi.UnsubscribeAll</c>) and drops the registered systems, so there is
    /// nothing for the mod to release itself.
    /// </summary>
    public void Unload()
    {
    }
}
