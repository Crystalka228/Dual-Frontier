namespace DualFrontier.Components.Shared;

using DualFrontier.Contracts.Attributes;
using DualFrontier.Contracts.Core;
using DualFrontier.Core.Interop;

/// <summary>
/// Represents the faction component for an entity, tracking its affiliation and hostility status.
/// </summary>
[ModAccessible(Read = true, Write = true)]
public struct FactionComponent : IComponent
{
    // Constants

    /// <summary>
    /// The unique string identifier used for the player's colony faction.
    /// Compile-time literal; intern via <c>NativeWorld.InternString</c> to
    /// produce an <see cref="InternedString"/> handle for comparison against
    /// the <see cref="FactionId"/> field.
    /// </summary>
    public const string PlayerFactionIdString = "colony";

    /// <summary>
    /// The unique string identifier used for neutral factions.
    /// Compile-time literal; intern via <c>NativeWorld.InternString</c>.
    /// </summary>
    public const string NeutralFactionIdString = "neutral";

    // Fields

    /// <summary>
    /// Interned handle for the faction this entity belongs to (e.g., "colony",
    /// "raiders", "neutral"). Empty sentinel = unaffiliated. Set at spawn time
    /// by the scenario loader via <c>NativeWorld.InternString</c>; compared
    /// against another faction's id by handle equality (same world) or
    /// <c>InternedString.EqualsByContent</c> (cross-world).
    /// </summary>
    public InternedString FactionId;

    /// <summary>
    /// Indicates whether this faction is currently hostile to the player colony.
    /// </summary>
    public bool IsHostile;

    /// <summary>
    /// Indicates whether this faction is directly controlled by the player.
    /// </summary>
    public bool IsPlayer;

    /// <inheritdoc />
    /// <summary>
    /// Checks if the entity's current faction status is neutral (not hostile and not the player).
    /// </summary>
    public bool IsNeutral => !IsHostile && !IsPlayer;
}
