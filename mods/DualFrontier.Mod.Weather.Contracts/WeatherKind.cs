namespace DualFrontier.Mod.Weather.Contracts;

/// <summary>
/// The weather vocabulary. MOD-OWNED: the engine has no idea these values exist, and no
/// engine assembly names this type. It lives in a SHARED mod so that every regular mod which
/// reacts to weather resolves the same <see cref="System.Type"/> through the shared
/// AssemblyLoadContext — a type defined inside a regular mod's collectible ALC would be
/// invisible to its neighbours (MOD_OS_ARCHITECTURE §5).
/// </summary>
public enum WeatherKind
{
    /// <summary>No weather effect. The scene renders untinted.</summary>
    Clear = 0,

    /// <summary>Steady rain.</summary>
    Rain = 1,

    /// <summary>Heavy storm.</summary>
    Storm = 2,

    /// <summary>Low visibility fog.</summary>
    Fog = 3,

    /// <summary>Snowfall.</summary>
    Snow = 4,

    /// <summary>Ether storm — the setting's own hazard, not a real-world weather kind.</summary>
    EtherStorm = 5,
}
