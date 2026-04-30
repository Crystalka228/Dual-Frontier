using System;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Modding;

namespace Fixture.BadRegularMod;

/// <summary>
/// Test fixture for D-4 enforcement: a regular mod whose assembly exports
/// types implementing <see cref="IEvent"/> and <see cref="IModContract"/>.
/// Phase E of <c>ContractValidator</c> must reject this mod before
/// <see cref="Initialize"/> runs. <see cref="Initialize"/> throws when invoked
/// so the timing invariant is observable to the integration test — if Phase E
/// were ever moved after Initialize, the resulting error message would change.
/// </summary>
public sealed class BadMod : IMod
{
    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        throw new InvalidOperationException(
            "BadMod.Initialize must never run — Phase E should reject this mod " +
            "before the pipeline reaches IMod.Initialize.");
    }

    /// <inheritdoc />
    public void Unload()
    {
    }
}

/// <summary>
/// Bad event type — defining <see cref="IEvent"/> in a regular mod is the
/// precise §6.5 D-4 violation Phase E catches.
/// </summary>
public sealed record BadEvent(int Value) : IEvent;

/// <summary>
/// Bad contract type — defining <see cref="IModContract"/> in a regular mod
/// is the precise §6.5 D-4 violation Phase E catches.
/// </summary>
public sealed record BadContract(string Id) : IModContract;
