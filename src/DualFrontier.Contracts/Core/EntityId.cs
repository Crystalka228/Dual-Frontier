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
    /// Returns <c>true</c> when this id was produced by <c>World.CreateEntity</c>
    /// and has not been explicitly set to <see cref="Invalid"/>.
    ///
    /// NOTE: <c>IsValid</c> only means the id is structurally non-default.
    /// It does NOT guarantee the entity is still alive — use
    /// <c>World.IsAlive(id)</c> for that check, which also compares versions.
    /// </summary>
    public bool IsValid => Index > 0 || Version > 0;
}