using DualFrontier.Contracts.Modding;

namespace Fixture.RegularMod_BadApiVersion;

/// <summary>
/// M5.2 fixture — regular mod whose v2 manifest declares
/// <c>apiVersion: "^99.0.0"</c>. The current build of
/// <c>DualFrontier.Contracts</c> is at <c>1.0.0</c>, so the modernized
/// Phase A path must reject the manifest with
/// <see cref="DualFrontier.Application.Modding.ValidationErrorKind.IncompatibleVersion"/>
/// (NOT the legacy <c>IncompatibleContractsVersion</c>) per
/// MOD_OS_ARCHITECTURE §11.2.
/// </summary>
public sealed class BadApiVersionMod : IMod
{
    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        // Empty — the test only exercises the Phase A v2 rejection path.
    }

    /// <inheritdoc />
    public void Unload()
    {
    }
}
