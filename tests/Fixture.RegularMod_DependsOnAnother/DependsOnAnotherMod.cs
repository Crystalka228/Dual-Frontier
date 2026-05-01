using DualFrontier.Contracts.Modding;

namespace Fixture.RegularMod_DependsOnAnother;

/// <summary>
/// M5.1 fixture — regular mod whose manifest declares a single required
/// dependency on <c>tests.regular.dependedon</c>. Used together with
/// <c>Fixture.RegularMod_DependedOn</c> to exercise the topological sort
/// pass: the helper must load the dependency first, then this mod.
/// </summary>
public sealed class DependsOnAnotherMod : IMod
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
