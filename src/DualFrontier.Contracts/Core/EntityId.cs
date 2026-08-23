using System;

namespace DualFrontier.Contracts.Core;

/// <summary>
/// Immutable entity identifier.
/// Consists of a slot <see cref="Index"/> in the ECS component arrays
/// and a <see cref="Version"/> that increments on every <c>DestroyEntity</c>
/// call for that slot.
///
/// Version-based validation: if an external system cached an <see cref="EntityId"/>
/// and the entity was subsequently destroyed and a new one recycled the same slot,
/// the new entity's version will be higher. Comparing
/// <c>id.Version == world.GetVersion(id.Index)</c> returns <c>false</c> for the
/// stale reference — safe skip, no crash.
///
/// Layout: two <c>int</c> fields → 8 bytes, aligns naturally in arrays and
/// component structs. Serialisation format is fixed; changing field types is
/// a breaking change for Save/Load.
/// </summary>
public readonly record struct EntityId(int Index, int Version)
{
    /// <summary>
    /// Sentinel "no entity" value — equivalent of <c>null</c> for reference types.
    /// Returned by <c>TryGet</c>-style methods when there is no result.
    /// <c>IsValid</c> returns <c>false</c> for this value.
    /// </summary>
    public static readonly EntityId Invalid = default;

    /// <summary>
    /// Returns <c>true</c> when this id could have been produced by
    /// <c>World.CreateEntity</c> — that is, when it names a real slot.
    ///
    /// <para>
    /// NOTE: <c>IsValid</c> only means the id is structurally addressable.
    /// It does NOT guarantee the entity is still alive — use
    /// <c>World.IsAlive(id)</c> for that check, which also compares versions.
    /// Current-generation knowledge is unknowable without a world call, so this
    /// stays a <i>syntactic</i> check by design.
    /// </para>
    ///
    /// <para>
    /// The check is the syntactic projection of the world's own rule, which
    /// rejects <c>index &lt;= 0</c> unconditionally: index 0 is the reserved
    /// <see cref="Invalid"/> slot and the world mints from 1 upward. It
    /// previously read <c>Index &gt; 0 || Version &gt; 0</c>, which called
    /// <c>(0, 5)</c> valid while the world held it permanently dead
    /// (ID-B / К-L22; IDENTITY_AND_ABI_CONTRACT §2 "IsValid alignment").
    /// The native mirror <c>entity_id.h::is_valid</c> carries the same rule.
    /// </para>
    /// </summary>
    public bool IsValid => Index > 0;
}