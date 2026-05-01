using DualFrontier.Contracts.Modding;

namespace Fixture.RegularMod_ReplacesCombat_Alt;

/// <summary>
/// M6.2 fixture — second mod also declaring <c>CombatSystem</c> in
/// <c>replaces</c>. Paired with <c>Fixture.RegularMod_ReplacesCombat</c>
/// to drive the BridgeReplacementConflict test scenario from
/// MOD_OS_ARCHITECTURE §7.2: two mods may not replace the same system in
/// the same load batch. <see cref="Initialize"/> is never reached —
/// Phase H rejects the batch before <c>IMod.Initialize</c> runs — so the
/// body throws to make any accidental invocation fail loudly.
/// </summary>
public sealed class ReplacesCombatAltMod : IMod
{
    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        throw new System.InvalidOperationException(
            "Fixture.RegularMod_ReplacesCombat_Alt.Initialize must never run — " +
            "Phase H is expected to reject the conflicting batch first.");
    }

    /// <inheritdoc />
    public void Unload()
    {
    }
}
