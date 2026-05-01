using DualFrontier.Contracts.Modding;

namespace Fixture.RegularMod_MissingOptional;

/// <summary>
/// M5.1 fixture — regular mod that declares an optional dependency on
/// <c>tests.regular.absent</c>, a mod that never appears in the load
/// batch. Optional missing deps must NOT block loading; the pipeline's
/// pass [0.6] presence check produces a non-blocking
/// <c>ValidationWarning</c> and the mod still reaches Initialize.
/// </summary>
public sealed class MissingOptionalMod : IMod
{
    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        // Empty — the test only exercises the warning flow.
    }

    /// <inheritdoc />
    public void Unload()
    {
    }
}
