using System;
using DualFrontier.Contracts.Modding;

namespace Fixture.RegularMod_CyclicA;

/// <summary>
/// M5.1 fixture — half of the regular-mod cycle test pair. Depends on
/// <c>tests.regular.cycb</c>, which in turn depends on this mod's id —
/// a 2-node cycle the M5.1 pipeline must detect at pass [0.6] and
/// reject before pass 2 (LoadRegularMod). Initialize must therefore
/// never run; if it does, the timing invariant is broken and the test
/// surfaces it through the InvalidOperationException message.
/// </summary>
public sealed class CyclicAMod : IMod
{
    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        throw new InvalidOperationException(
            "CyclicAMod.Initialize must never run — pass [0.6] should reject " +
            "this mod before the pipeline reaches IMod.Initialize.");
    }

    /// <inheritdoc />
    public void Unload()
    {
    }
}
