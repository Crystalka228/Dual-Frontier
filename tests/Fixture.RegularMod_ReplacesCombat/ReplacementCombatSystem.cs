using System;
using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Bus;
using DualFrontier.Contracts.Sdk;

namespace Fixture.RegularMod_ReplacesCombat;

/// <summary>
/// M6.2 fixture — minimal stand-in for the kernel <c>CombatSystem</c> when a mod
/// replaces it. Carries the attributes <c>ModRegistry.RegisterSystem</c> requires
/// (<c>[SystemAccess]</c> + <c>[TickRate]</c>) but declares no writes, so Phase B
/// (<c>ContractValidator.ValidateWriteWriteConflicts</c>) stays silent. The test
/// asserts registration / skip mechanics, not gameplay fidelity.
///
/// W1 (SDK unlock): retargeted from <c>SystemBase</c> to the SDK contract
/// <see cref="ISimulationSystem"/>. This is the empirical BD-1 gap-closure proof —
/// the fixture no longer needs a <c>DualFrontier.Core</c> project reference to name
/// a base type; it is authored against <c>DualFrontier.Contracts</c> alone. The
/// engine wraps it onto the executor through the internal SystemAdapter.
/// </summary>
[SystemAccess(
    reads:  new Type[0],
    writes: new Type[0],
    bus:    nameof(IGameServices.Combat))]
[TickRate(TickRates.NORMAL)]
public sealed class ReplacementCombatSystem : ISimulationSystem
{
    public void Initialize(ISystemContext context)
    {
    }

    public void Tick(ISystemContext context)
    {
    }

    public void OnDispose()
    {
    }
}
