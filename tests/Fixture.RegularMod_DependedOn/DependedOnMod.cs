using DualFrontier.Contracts.Modding;

namespace Fixture.RegularMod_DependedOn;

/// <summary>
/// M5.1 fixture — regular mod with no declared dependencies. Used as the
/// dependency target for <c>Fixture.RegularMod_DependsOnAnother</c>; the
/// topological sort pass must place this mod ahead of its dependent.
/// </summary>
public sealed class DependedOnMod : IMod
{
    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        // Empty — the test only needs the mod to be loadable.
    }

    /// <inheritdoc />
    public void Unload()
    {
    }
}
