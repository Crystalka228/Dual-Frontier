using DualFrontier.Contracts.Modding;

namespace Fixture.RegularMod_ReplacesProtected;

/// <summary>
/// M6.2 fixture — regular mod attempting to replace
/// <c>DualFrontier.Systems.Pawn.SocialSystem</c>, which is annotated
/// <c>[BridgeImplementation(Phase = 3)]</c> with the default
/// <c>Replaceable=false</c>. Phase H must reject the batch with
/// <c>ProtectedSystemReplacement</c>; <see cref="Initialize"/> is never
/// reached, hence the throw — accidental invocation must fail loudly so
/// the integration test catches an order-of-operations regression.
/// </summary>
public sealed class ReplacesProtectedMod : IMod
{
    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        throw new System.InvalidOperationException(
            "Fixture.RegularMod_ReplacesProtected.Initialize must never run — " +
            "Phase H is expected to reject the protected-replacement batch first.");
    }

    /// <inheritdoc />
    public void Unload()
    {
    }
}
