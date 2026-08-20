using System;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Sdk;
using DualFrontier.Mod.Weather.Contracts;

namespace DualFrontier.Mod.Weather;

/// <summary>
/// Advances world weather on a fixed cadence and announces every transition. This is the
/// simulation half of the mechanic; <see cref="WeatherPresentationSystem"/> is the visual half.
///
/// <para>
/// <b>Determinism.</b> There is no wall clock, no <c>Random</c>, and no state carried between
/// ticks beyond the component itself. The next weather is a PURE FUNCTION of
/// (SimTick, current kind) hashed against a compile-time seed, so the same tick history always
/// produces the same weather history — on any machine, in any process, in a replay.
/// </para>
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new[] { typeof(WeatherStateComponent) })]
[TickRate(TickRates.NORMAL)]
public sealed class WeatherSystem : ISimulationSystem
{
    /// <summary>SimTicks between weather transitions.</summary>
    public const long TransitionPeriodTicks = 300;

    /// <summary>Compile-time seed. Changing it changes every weather history this mod produces.</summary>
    private const ulong Seed = 0x9E3779B97F4A7C15UL;

    private const int KindCount = 6;

    /// <summary>
    /// Non-Clear weather never drops below this severity, so a transition is always VISIBLE.
    /// Without the floor a legitimately-hashed 0.01 storm would render as no storm at all, and
    /// the mechanic would look broken rather than subtle.
    /// </summary>
    private const float MinimumIntensity = 0.35f;

    /// <summary>Nothing to set up: this system owns no subscriptions and no cached state.</summary>
    public void Initialize(ISystemContext context)
    {
    }

    /// <inheritdoc />
    public void Tick(ISystemContext context)
    {
        if (!TryReadSingleton(context, out EntityId entity, out WeatherStateComponent state))
        {
            SeedSingleton(context);
            return;
        }

        long tick = context.CurrentTick;
        if (tick - state.LastTransitionTick < TransitionPeriodTicks)
            return;

        var previous = (WeatherKind)state.Kind;
        (WeatherKind next, float intensity) = NextState(tick, previous);

        using (WriteScope<WeatherStateComponent> batch = context.BeginBatch<WeatherStateComponent>())
        {
            batch.Update(entity, new WeatherStateComponent
            {
                Kind = (int)next,
                Intensity = intensity,
                LastTransitionTick = tick,
            });
        }

        // Published AFTER the batch scope closes, so the flush has landed and any subscriber
        // that reads the component sees the weather the event announces. A cross-owner publish:
        // the event type belongs to the shared contracts mod, so this mod's manifest declares
        // mod.dualfrontier.weather.contracts.publish:<FQN> and the gate enforces it strictly.
        context.Publish(new WeatherChangedEvent
        {
            Kind = next,
            PreviousKind = previous,
            Intensity = intensity,
        });
    }

    /// <summary>Nothing to release — no subscriptions, no unmanaged handles.</summary>
    public void OnDispose()
    {
    }

    /// <summary>
    /// Reads the weather singleton if it exists. The span is released before this returns, which
    /// matters: the world rejects mutation while a span is live, so the caller must not be
    /// holding one when it mints an entity or opens a write batch.
    /// </summary>
    private static bool TryReadSingleton(
        ISystemContext context,
        out EntityId entity,
        out WeatherStateComponent state)
    {
        entity = default;
        state = default;

        using SpanScope<WeatherStateComponent> span = context.AcquireSpan<WeatherStateComponent>();
        foreach ((EntityId candidate, WeatherStateComponent value) in span.Pairs)
        {
            entity = candidate;
            state = value;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Mints the weather singleton in its initial Clear state. Idempotent by construction: the
    /// caller only reaches here when no singleton exists, and on a RELOAD the existing entity is
    /// adopted instead (the component survives the mod's ALC — see the G3 finding), so a reloaded
    /// mod resumes the world's weather rather than resetting it.
    /// </summary>
    private static void SeedSingleton(ISystemContext context)
    {
        EntityId singleton = context.CreateEntity();

        using WriteScope<WeatherStateComponent> batch = context.BeginBatch<WeatherStateComponent>();
        batch.Add(singleton, new WeatherStateComponent
        {
            Kind = (int)WeatherKind.Clear,
            Intensity = 0f,
            LastTransitionTick = context.CurrentTick,
        });
    }

    /// <summary>
    /// The transition law: a pure hash of (tick, current kind). Never returns the current kind,
    /// so every transition is an actual change rather than a silent no-op the player cannot see.
    /// </summary>
    internal static (WeatherKind Kind, float Intensity) NextState(long tick, WeatherKind current)
    {
        ulong h = Mix(Seed ^ Mix(unchecked((ulong)tick)) ^ (ulong)(uint)(int)current);

        // Offset by 1..KindCount-1 so the result is always a DIFFERENT kind.
        int step = 1 + (int)(h % (KindCount - 1));
        var next = (WeatherKind)(((int)current + step) % KindCount);

        if (next == WeatherKind.Clear)
            return (next, 0f);

        float raw = ((h >> 32) & 0xFFFF) / 65535f;
        return (next, MinimumIntensity + ((1f - MinimumIntensity) * raw));
    }

    /// <summary>splitmix64 finalizer — a fixed avalanche function, not a random-number generator.</summary>
    private static ulong Mix(ulong x)
    {
        unchecked
        {
            x ^= x >> 30;
            x *= 0xBF58476D1CE4E5B9UL;
            x ^= x >> 27;
            x *= 0x94D049BB133111EBUL;
            x ^= x >> 31;
            return x;
        }
    }
}
