using System;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Core.ECS;

namespace Fixture.RegularMod_ReplacesCombat;

/// <summary>
/// M6.2 fixture — minimal stand-in for the kernel <c>CombatSystem</c>
/// when a mod replaces it. Carries the attributes
/// <c>ModRegistry.RegisterSystem</c> requires (<c>[SystemAccess]</c> +
/// <c>[TickRate]</c>) but declares no writes — that keeps Phase B
/// (<c>ContractValidator.ValidateWriteWriteConflicts</c>) silent, since
/// the kernel <c>CombatSystem</c> stays in the registry as a Core-origin
/// entry and its <c>HealthComponent</c> writes would otherwise collide
/// with any same-write replacement. Phase B does not (in M6) consult
/// <c>Manifest.Replaces</c> — that ratification belongs in a later
/// phase if the gap matters; for now the fixture sidesteps it by being
/// structurally minimal. The test asserts registration / skip mechanics,
/// not gameplay surface fidelity.
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new Type[0],
    bus:    nameof(IGameServices.Combat))]
[TickRate(TickRates.NORMAL)]
public sealed class ReplacementCombatSystem : SystemBase
{
    public override void Update(float delta)
    {
    }
}
