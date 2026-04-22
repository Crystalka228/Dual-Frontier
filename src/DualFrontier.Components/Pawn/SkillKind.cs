namespace DualFrontier.Components.Pawn;

/// <summary>
/// Defines the core skill types for a pawn in the colony simulation.
/// </summary>
public enum SkillKind
{
    /// <summary>
    /// Proficiency in building and repairing structures.
    /// </summary>
    Construction,

    /// <summary>
    /// Efficiency in extracting ore, stone, and other raw resources from terrain.
    /// </summary>
    Mining,

    /// <summary>
    /// Skill related to preparing food, brewing consumables, and general culinary arts.
    /// </summary>
    Cooking,

    /// <summary>
    /// General competence in crafting items using workbenches and specialized tools.
    /// </summary>
    Crafting,

    /// <summary>
    /// Effectiveness in treating injuries, managing diseases, and performing medical procedures.
    /// </summary>
    Medicine,

    /// <summary>
    /// Proficiency in combat, covering both melee weaponry and ranged fighting effectiveness.
    /// </summary>
    Combat,

    /// <summary>
    /// Skill focused on ranged weapon accuracy, maintenance, and reload speed.
    /// </summary>
    Shooting,

    /// <summary>
    /// The ability to cast arcane spells, manage mana resources, and general magical power.
    /// </summary>
    Magic,

    /// <summary>
    /// Speed and proficiency in conducting industrial technology research.
    /// </summary>
    Research,

    /// <summary>
    /// Skill related to trade, persuasion, diplomacy, and social interactions with other pawns or factions.
    /// </summary>
    Social,

    /// <summary>
    /// Aptitude for taming, breeding, and training various types of creatures.
    /// </summary>
    Animals,

    /// <summary>
    /// Skill concerning growing crops, cultivating plants, and general agricultural practices.
    /// </summary>
    Farming,

    /// <summary>
    /// Measures carry capacity and the speed bonus when hauling resources or items.
    /// </summary>
    Hauling
}