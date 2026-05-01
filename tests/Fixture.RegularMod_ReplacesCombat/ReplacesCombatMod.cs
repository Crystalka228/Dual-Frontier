using DualFrontier.Contracts.Modding;

namespace Fixture.RegularMod_ReplacesCombat;

/// <summary>
/// M6.2 fixture — regular mod whose manifest replaces the kernel
/// <c>CombatSystem</c> bridge. <see cref="Initialize"/> registers
/// <see cref="ReplacementCombatSystem"/>, which the pipeline
/// (per MOD_OS_ARCHITECTURE §7.1 step 3) puts into the dependency graph in
/// place of the kernel system. The integration test asserts that after
/// <c>Apply</c> succeeds, the kernel <c>CombatSystem</c> is absent from the
/// scheduler's phase list and <see cref="ReplacementCombatSystem"/> is
/// present.
/// </summary>
public sealed class ReplacesCombatMod : IMod
{
    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        api.RegisterSystem<ReplacementCombatSystem>();
    }

    /// <inheritdoc />
    public void Unload()
    {
    }
}
