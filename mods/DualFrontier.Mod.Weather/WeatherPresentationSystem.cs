using System;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Sdk;
using DualFrontier.Mod.Weather.Contracts;

namespace DualFrontier.Mod.Weather;

/// <summary>
/// Turns a weather transition into a whole-scene colour. The visual half of the mechanic: it
/// reads and writes NO components, it only listens and paints.
///
/// <para>
/// <b>Cross-owner subscribe.</b> <c>WeatherChangedEvent</c> belongs to the shared contracts mod,
/// so this mod manifest declares
/// <c>mod.dualfrontier.weather.contracts.subscribe:&lt;FQN&gt;</c> and the capability gate
/// enforces it strictly -- this mod declares capabilities, so it left the v1 grace path.
/// </para>
///
/// <para>
/// <b>Why capturing the context here is safe.</b> The freshness law forbids holding the context
/// across ticks for WORLD access, because a cached world view outlives graph rebuilds that
/// invalidate it. <c>SetAmbientTint</c> touches no world state at all -- it hands a colour to the
/// engine presentation sink, which enqueues a render command -- so the captured reference carries
/// nothing that can go stale. It is additionally safe because the engine re-pushes the subscribing
/// system execution context around every delivery. Do NOT copy this pattern for component access.
/// </para>
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new Type[0])]
[TickRate(TickRates.NORMAL)]
public sealed class WeatherPresentationSystem : ISimulationSystem
{
    /// <summary>
    /// Per-kind base tint: colour plus the strength applied at full intensity. Mod data -- the
    /// engine has no opinion about what a storm looks like.
    /// </summary>
    internal static readonly (float R, float G, float B, float Strength)[] TintTable =
    {
        /* Clear      */ (1.00f, 1.00f, 1.00f, 0.00f),
        /* Rain       */ (0.55f, 0.70f, 0.95f, 0.55f),
        /* Storm      */ (0.30f, 0.34f, 0.45f, 0.80f),
        /* Fog        */ (0.80f, 0.82f, 0.85f, 0.60f),
        /* Snow       */ (0.88f, 0.94f, 1.00f, 0.50f),
        /* EtherStorm */ (0.72f, 0.35f, 0.95f, 0.85f),
    };

    // Guards against re-subscribing. The scheduler calls Initialize on every graph REBUILD, not
    // once per instance, so a mod that subscribes here would accumulate a duplicate subscription
    // each time another mod loads or unloads -- and every weather change would then be painted
    // twice. Engine-side behaviour, mod-side defence.
    private bool _subscribed;

    /// <inheritdoc />
    public void Initialize(ISystemContext context)
    {
        if (_subscribed)
            return;
        _subscribed = true;

        context.Subscribe<WeatherChangedEvent>(evt =>
        {
            (float r, float g, float b, float strength) = TintFor(evt.Kind);

            // Intensity scales the base strength, so a mild rain tints less than a downpour.
            // Clear carries base strength 0, so Clear always restores the untinted scene exactly.
            context.SetAmbientTint(r, g, b, strength * evt.Intensity);
        });
    }

    /// <summary>Nothing per-tick: this system is purely event-driven.</summary>
    public void Tick(ISystemContext context)
    {
    }

    /// <summary>
    /// Teardown. The engine unload chain releases the subscription itself
    /// (<c>RestrictedModApi.UnsubscribeAll</c>, MOD_OS §9.5 step 1); clearing the guard here
    /// keeps a re-initialised instance able to subscribe again.
    /// </summary>
    public void OnDispose()
    {
        _subscribed = false;
    }

    /// <summary>Looks up the base tint for a kind, falling back to no-tint for an unknown value.</summary>
    internal static (float R, float G, float B, float Strength) TintFor(WeatherKind kind)
    {
        int index = (int)kind;
        return index >= 0 && index < TintTable.Length
            ? TintTable[index]
            : TintTable[(int)WeatherKind.Clear];
    }
}
