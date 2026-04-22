namespace DualFrontier.Components.Pawn;

/// <summary>
/// Defines the possible job assignments a pawn can undertake in the colony.
/// </summary>
public enum JobKind
{
    /// <summary>
    /// The pawn has no assigned task or is resting/idle.
    /// </summary>
    Idle,

    /// <summary>
    /// Pawn is carrying items from a source to a destination point.
    /// </summary>
    Haul,

    /// <summary>
    /// Constructing or repairing a building or structure in the colony.
    /// </summary>
    Build,

    /// <summary>
    /// Extracting resources (ore/stone) from designated terrain points.
    /// </summary>
    Mine,

    /// <summary>
    /// Preparing food at a stove, campfire, or other culinary station.
    /// </summary>
    Cook,

    /// <summary>
    /// Producing items using various tools and workbenches.
    /// </summary>
    Craft,

    /// <summary>
    /// Working specifically at an advanced research bench to advance technology.
    /// </summary>
    Research,

    /// <summary>
    /// Treating an injured or sick pawn requiring medical attention.
    /// </summary>
    Medicate,

    /// <summary>
    /// Engaging a designated target in combat for direct conflict resolution.
    /// </summary>
    Fight,

    /// <summary>
    /// Retreating from danger or moving away from immediate threat zones.
    /// </summary>
    Flee,

    /// <summary>
    /// Practicing arcane meditation to regenerate mana or ether reserves.
    /// </summary>
    Meditate,

    /// <summary>
    /// Managing, issuing commands to, or maintaining a golem construct.
    /// </summary>
    GolemCommand,

    /// <summary>
    /// Resting at a designated bed or on the ground to recover stamina/health.
    /// </summary>
    Sleep,

    /// <summary>
    /// Consuming food items from inventory or designated tables for sustenance.
    /// </summary>
    Eat,

    /// <summary>
    /// Initiating social interaction (e.g., trade, conversation) with another pawn.
    /// </summary>
    Social
}