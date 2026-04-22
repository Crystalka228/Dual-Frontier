namespace DualFrontier.Components.Shared;

using DualFrontier.Contracts.Core;

/// <summary>
/// Represents the faction component for an entity, tracking its affiliation and hostility status.
/// </summary>
public sealed class FactionComponent : IComponent
{
    // Constants
    
    /// <summary>
    /// The unique string identifier used for the player's colony faction.
    /// </summary>
    public const string PlayerFactionId = "colony";

    /// <summary>
    /// The unique string identifier used for neutral factions.
    /// </summary>
    public const string NeutralFactionId = "neutral";

    // Fields
    
    /// <summary>
    /// A unique string identifier of the faction this entity belongs to (e.g., "colony", "raiders", "neutral").
    /// </summary>
    public string FactionId { get; init; } = "";
    
    /// <summary>
    /// Indicates whether this faction is currently hostile to the player colony.
    /// </summary>
    public bool IsHostile { get; init; }
    
    /// <summary>
    /// Indicates whether this faction is directly controlled by the player.
    /// </summary>
    public bool IsPlayer { get; init; }

    /// <inheritdoc />
    /// <summary>
    /// Checks if the entity's current faction status is neutral (not hostile and not the player).
    /// </summary>
    public bool IsNeutral => !IsHostile && !IsPlayer;
}