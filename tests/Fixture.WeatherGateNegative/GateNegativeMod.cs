using DualFrontier.Contracts.Modding;
using DualFrontier.Mod.Weather.Contracts;

namespace Fixture.WeatherGateNegative;

/// <summary>
/// W3 wave-gate NEGATIVE fixture. Its manifest declares capabilities (so it is OUT of the v1
/// grace path and strictly gated) and declares the SUBSCRIBE token for WeatherChangedEvent --
/// but not PUBLISH. It then publishes anyway, from Initialize, so the violation surfaces through
/// the real pipeline as a load failure rather than needing a tick.
///
/// It passes Phase C: the token it DOES require is owned by a declared dependency and held by the
/// ledger. The rejection it earns comes from the RUNTIME gate, which is the point -- the two
/// checks are independent and this fixture proves the second one alone.
/// </summary>
public sealed class GateNegativeMod : IMod
{
    public void Initialize(IModApi api)
    {
        api.Publish(new WeatherChangedEvent
        {
            Kind = WeatherKind.Storm,
            PreviousKind = WeatherKind.Clear,
            Intensity = 1f,
        });
    }

    public void Unload()
    {
    }
}
