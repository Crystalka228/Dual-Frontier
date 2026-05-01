using DualFrontier.Contracts.Modding;

namespace Fixture.RegularMod_DepsBadVersion;

/// <summary>
/// M5.2 fixture — regular mod that depends on
/// <c>tests.regular.dependedon</c> (which exists at version <c>1.0.0</c>)
/// with a mismatched constraint <c>^99.0.0</c>. Phase G must reject the
/// load with
/// <see cref="DualFrontier.Application.Modding.ValidationErrorKind.IncompatibleVersion"/>
/// attributed to this mod (the dependent — its constraint is what failed)
/// per MOD_OS_ARCHITECTURE §8.7.
/// </summary>
public sealed class DepsBadVersionMod : IMod
{
    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        // Empty — the test only exercises the Phase G rejection path.
    }

    /// <inheritdoc />
    public void Unload()
    {
    }
}
