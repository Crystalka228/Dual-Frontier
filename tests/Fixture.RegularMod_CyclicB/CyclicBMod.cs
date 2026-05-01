using System;
using DualFrontier.Contracts.Modding;

namespace Fixture.RegularMod_CyclicB;

/// <summary>
/// M5.1 fixture — second half of the regular-mod cycle test pair. See
/// <c>Fixture.RegularMod_CyclicA</c> for the cycle invariant.
/// </summary>
public sealed class CyclicBMod : IMod
{
    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        throw new InvalidOperationException(
            "CyclicBMod.Initialize must never run — pass [0.6] should reject " +
            "this mod before the pipeline reaches IMod.Initialize.");
    }

    /// <inheritdoc />
    public void Unload()
    {
    }
}
