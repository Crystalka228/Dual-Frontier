using System;
using DualFrontier.Contracts.Modding;

namespace Fixture.BadSharedMod_WithIMod;

/// <summary>
/// Test fixture for ContractValidator Phase F: a shared mod whose
/// assembly contains an <see cref="IMod"/> implementation. Per
/// MOD_OS_ARCHITECTURE §5.2 shared mods are pure type vendors and must
/// not declare entry points; Phase F surfaces this as a typed
/// <c>SharedModWithEntryPoint</c> error naming this type by FQN.
/// <see cref="Initialize"/> throws when invoked so the architectural
/// invariant is observable to integration tests — if Phase F were ever
/// moved past Initialize, the resulting error message would change.
/// </summary>
public sealed class WronglyHere : IMod
{
    /// <inheritdoc />
    public void Initialize(IModApi api)
    {
        throw new InvalidOperationException(
            "WronglyHere.Initialize must never run — Phase F should reject " +
            "the shared mod that hosts this type before any IMod entry point " +
            "is exercised.");
    }

    /// <inheritdoc />
    public void Unload()
    {
    }
}

/// <summary>
/// A legitimate shared type co-resident with the bad <see cref="WronglyHere"/>
/// IMod implementation. Demonstrates that Phase F flags the IMod presence
/// even when the assembly otherwise looks like a normal shared-mod
/// type-vendor.
/// </summary>
public sealed record SomeSharedType(string Name);
