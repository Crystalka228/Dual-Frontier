using DualFrontier.Contracts.Modding;

namespace Fixture.RegularMod_MissingRequired;

/// <summary>
/// M5.1 fixture — regular mod that declares a required dependency on
/// <c>tests.regular.absent</c>, a mod that never appears in the load
/// batch. The pipeline's pass [0.6] dependency presence check must
/// surface a <c>ValidationErrorKind.MissingDependency</c> for this mod.
/// </summary>
public sealed class MissingRequiredMod : IMod
{
    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        // Empty — the test only exercises the load and presence-check paths.
    }

    /// <inheritdoc />
    public void Unload()
    {
    }
}
