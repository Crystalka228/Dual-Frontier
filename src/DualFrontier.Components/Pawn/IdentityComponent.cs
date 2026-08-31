namespace DualFrontier.Components.Pawn;

using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Contracts.Sdk;

/// <summary>
/// Pure POCO carrying a pawn's identity data. Currently a single field —
/// <see cref="Name"/> — populated by <c>RandomPawnFactory</c> (or any
/// future scenario loader). Pawns lacking this component carry no name;
/// <c>PawnStateReporterSystem</c> publishes empty <c>Name</c> in that
/// case. The UI displays empty name verbatim — no fabricated fallback.
/// </summary>
[ModAccessible(Read = true, Write = true)]
public struct IdentityComponent : IComponent
{
    /// <summary>
    /// Display name handle. Default = the empty sentinel
    /// (<see cref="StringHandle.Empty"/>). Set at spawn time by the
    /// scenario factory via <c>NativeWorld.InternHandle</c>; not mutated
    /// thereafter except by future rename systems. Resolve at the
    /// presentation site via <c>NativeWorld.Resolve(StringHandle)</c> or
    /// <c>ISystemContext.Resolve</c>; the empty sentinel resolves to
    /// <c>null</c> per the «no fabricated fallback» rule — callers convert
    /// <c>null</c> to empty string at the display boundary.
    ///
    /// <para>
    /// W4 re-typed this from the <c>Core.Interop</c> wrapper to the Contracts-side handle. The
    /// payload is identical — an <c>(Id, Generation)</c> pair — but the handle is a type a mod
    /// may name, which the wrapper is not, and the vanilla scenario is authored from a mod from
    /// W4 onward. Managed-only: the kernel stores this field's bytes and never interprets them.
    /// </para>
    /// </summary>
    public StringHandle Name;
}
