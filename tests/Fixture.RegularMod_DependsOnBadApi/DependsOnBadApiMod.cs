using DualFrontier.Contracts.Modding;

namespace Fixture.RegularMod_DependsOnBadApi;

/// <summary>
/// M5.2 fixture — regular mod paired with
/// <c>Fixture.RegularMod_BadApiVersion</c> for the cascade-failure
/// scenario. This mod's manifest declares a dependency on
/// <c>tests.regular.badapi</c> with version constraint <c>^99.0.0</c>,
/// while <c>tests.regular.badapi</c>'s declared version is <c>1.0.0</c>.
/// Both errors must surface independently:
/// <list type="bullet">
///   <item><c>tests.regular.badapi</c> fails Phase A with IncompatibleVersion (its v2 apiVersion=^99.0.0 is unsatisfied by the current Contracts).</item>
///   <item><c>tests.regular.donbadapi</c> fails Phase G with IncompatibleVersion (its dep constraint ^99.0.0 is unsatisfied by the provider's manifest.Version=1.0.0).</item>
/// </list>
/// </summary>
public sealed class DependsOnBadApiMod : IMod
{
    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        // Empty — the test only exercises the cascade-failure surfacing.
    }

    /// <inheritdoc />
    public void Unload()
    {
    }
}
