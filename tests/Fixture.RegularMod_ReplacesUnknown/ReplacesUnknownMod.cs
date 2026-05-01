using DualFrontier.Contracts.Modding;

namespace Fixture.RegularMod_ReplacesUnknown;

/// <summary>
/// M6.2 fixture — regular mod listing a fully-qualified type name in
/// <c>replaces</c> that resolves to no type in any loaded assembly. Phase
/// H must reject the batch with <c>UnknownSystemReplacement</c>;
/// <see cref="Initialize"/> never runs and throws on accidental
/// invocation.
/// </summary>
public sealed class ReplacesUnknownMod : IMod
{
    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        throw new System.InvalidOperationException(
            "Fixture.RegularMod_ReplacesUnknown.Initialize must never run — " +
            "Phase H is expected to reject the unknown-FQN batch first.");
    }

    /// <inheritdoc />
    public void Unload()
    {
    }
}
